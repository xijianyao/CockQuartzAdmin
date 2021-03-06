﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using CockQuartz.Interface;
using CockQuartz.Model;
using FeI.Application;
using FeI.Domain.Repositories;
using Quartz;
using Quartz.Impl.Matchers;
using eHi.Common.Dto.Paged;

namespace CockQuartz.Application
{
    public class JobService : ApplicationService, IJobService
    {
        private readonly IScheduler _scheduler;
        private readonly IRepository<JobDetail> _jobDetailRepository;
        private readonly IRepository<JobExecuteLogs> _jobExecuteLogsRepository;
        private readonly CockQuartzDbContext _dbContext;
        private readonly string _quartzInstanceName = ConfigurationManager.AppSettings["QuartzInstanceName"];

        public JobService(IRepository<JobDetail> jobDetailRepository,
            IRepository<JobExecuteLogs> jobExecuteLogsRepository)
        {
            _jobDetailRepository = jobDetailRepository;
            _scheduler = SchedulerManager.Instance;
            _dbContext = DbContextFactory.DbContext;
            _jobExecuteLogsRepository = jobExecuteLogsRepository;
        }

        public int CreateJob(JobDetail job)
        {
            job.CreateTime = DateTime.Now;
            return _jobDetailRepository.InsertAndGetId(job);
        }

        /// <summary>
        /// /获取任务列表
        /// </summary>
        /// <returns></returns>
        public PagedResultDto<JobDetailOutputDto> GetJobList(int pageIndex, string groupNames = "")
        {
            if (_scheduler == null)
            {
                return null;
            }

            var jobList = _jobDetailRepository.Query().AsNoTracking()
                .Where(x => !x.IsDeleted).ToList();
            if (!string.IsNullOrEmpty(groupNames))
            {
                jobList = jobList.Where(x => x.JobGroupName == groupNames).ToList();
            }
            var totalCount = jobList.Count;

            var result = new List<JobDetailOutputDto>();
            foreach (var groupName in _scheduler.GetJobGroupNames().Result)
            {
                foreach (JobKey jobKey in _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)).Result)
                {
                    var triggers = _scheduler.GetTriggersOfJob(jobKey).Result.ToList();
                    foreach (var item in triggers)
                    {
                        var jobViewModel = new JobDetailOutputDto
                        {
                            JobName = jobKey.Name,
                            JobGroupName = jobKey.Group,
                            TriggerGroupName = item.Key.Group,
                            TriggerName = item.Key.Name,
                            CurrentStatus = GetJobStatusByKey(item.Key),
                            NextRunTime = item.GetNextFireTimeUtc() == null
                            ? "下次不会触发运行时间"
                            : item.GetNextFireTimeUtc()?.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            LastRunTime = item.GetPreviousFireTimeUtc() == null
                            ? "还未运行过"
                            : item.GetPreviousFireTimeUtc()?.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff")
                        };

                        var jobInfo = jobList.FirstOrDefault(x =>
                            x.JobName == jobKey.Name && x.JobGroupName == jobKey.Group);
                        if (jobInfo == null)
                        {
                            continue;
                        }//
                        jobViewModel.Cron = jobInfo.Cron;
                        jobViewModel.RequestUrl = jobInfo.RequestUrl;
                        jobViewModel.ExceptionEmail = jobInfo.ExceptionEmail;
                        jobViewModel.Id = jobInfo.Id;
                        jobViewModel.IsInSchedule = true;
                        result.Add(jobViewModel);
                    }
                }
            }


            foreach (var item in result)
            {
                var jobInfo = jobList.FirstOrDefault(x => x.JobName == item.JobName && x.JobGroupName == item.JobGroupName);
                if (jobInfo != null)
                {
                    jobList.Remove(jobInfo);
                }
            }

            if (jobList.Count > 0)
            {
                foreach (var item in jobList)
                {
                    var jobViewModel = new JobDetailOutputDto(); ;
                    jobViewModel.JobName = item.JobName;
                    jobViewModel.JobGroupName = item.JobGroupName;
                    jobViewModel.TriggerGroupName = item.TriggerGroupName;
                    jobViewModel.TriggerName = item.TriggerGroupName;
                    jobViewModel.CurrentStatus = "还未加入计划中";
                    jobViewModel.NextRunTime = "无";
                    jobViewModel.LastRunTime = "无";
                    jobViewModel.Cron = item.Cron;
                    jobViewModel.Id = item.Id;
                    jobViewModel.IsInSchedule = false;
                    jobViewModel.RequestUrl = item.RequestUrl;
                    jobViewModel.ExceptionEmail = item.ExceptionEmail;
                    result.Add(jobViewModel);
                }
            }


            result = result.Skip(10 * (pageIndex - 1)).Take(pageIndex * 10).ToList();//分页默认每页大小为10
            return new PagedResultDto<JobDetailOutputDto>(totalCount, result);
            //return result;
        }

        /// <summary>
        /// 获取执行日子
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public List<JobExecuteLogs> GetJobLogList(int jobId)
        {
            return _jobExecuteLogsRepository.Query().AsNoTracking()
                .Where(x => x.JobDetailId == jobId)
                .OrderBy(x => x.Id).Take(50).ToList();
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="id">任务信息</param>
        /// <returns></returns>
        public bool RunJob(int id)
        {
            var jobInfo = _jobDetailRepository.FirstOrDefault(x => x.Id == id);

            JobKey jobKey = CreateJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            if (!_scheduler.CheckExists(jobKey).Result)
            {
                IJobDetail job = JobBuilder.Create<JobBase>()
                    .WithIdentity(jobKey)
                    //.UsingJobData(CreateJobDataMap("jobId", jobInfo.Id))
                    //.UsingJobData(CreateJobDataMap("requestUrl", jobInfo.RequestUrl))//添加此任务请求地址附带到Context上下文中
                    //.RequestRecovery(true)
                    .WithDescription(jobInfo.Description)
                    .Build();

                JobDataMap map = job.JobDataMap;
                map.Put("jobId", jobInfo.Id);

                CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(jobInfo.Cron);
                ITrigger trigger = TriggerBuilder.Create().StartNow()//StartAt(DateTime.SpecifyKind(jobInfo.JobStartTime, DateTimeKind.Local))
                    .WithIdentity(jobInfo.TriggerName, jobInfo.TriggerGroupName)
                    .ForJob(jobKey)
                    .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionDoNothing())
                    .WithDescription(jobInfo.Description)
                    .Build();
                #region Quartz 任务miss之后三种操作
                /*
             withMisfireHandlingInstructionDoNothing
——不触发立即执行
——等待下次Cron触发频率到达时刻开始按照Cron频率依次执行

withMisfireHandlingInstructionIgnoreMisfires
——以错过的第一个频率时间立刻开始执行
——重做错过的所有频率周期后
——当下一次触发频率发生时间大于当前时间后，再按照正常的Cron频率依次执行

withMisfireHandlingInstructionFireAndProceed
——以当前时间为触发频率立刻触发一次执行
——然后按照Cron频率依次执行*/
                #endregion

                _scheduler.ScheduleJob(job, trigger);

            }
            return true;

        }

        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="id">任务信息</param>
        /// <returns></returns>

        public bool DeleteJob(int id)
        {
            var jobInfo = _jobDetailRepository.FirstOrDefault(x => x.Id == id);

            var jobKey = CreateJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            var triggerKey = CreateTriggerKey(jobInfo.TriggerName, jobInfo.TriggerGroupName);
            _scheduler.PauseTrigger(triggerKey);
            _scheduler.UnscheduleJob(triggerKey);
            _scheduler.DeleteJob(jobKey);
            _jobDetailRepository.Delete(jobInfo);
            return true;
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="id">任务信息</param>
        /// <returns></returns>
        public bool PauseJob(int id)
        {
            var jobInfo = _jobDetailRepository.FirstOrDefault(x => x.Id == id);
            var jobKey = CreateJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            _scheduler.PauseJob(jobKey);
            return true;
        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <returns></returns>
        public bool ResumeJob(int id)
        {
            var jobInfo = _jobDetailRepository.FirstOrDefault(x => x.Id == id);
            var jobKey = CreateJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            _scheduler.ResumeJob(jobKey);
            return true;
        }

        /// <summary>
        /// 立即执行
        /// </summary>
        /// <returns></returns>
        public bool StartJob(int id)
        {
            var jobInfo = _jobDetailRepository.FirstOrDefault(x => x.Id == id);

            CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(jobInfo.Cron);
            var triggerKey = CreateTriggerKey(jobInfo.TriggerName, jobInfo.TriggerGroupName);
            ITrigger trigger = TriggerBuilder.Create().StartNow()
                .WithIdentity(jobInfo.TriggerName, jobInfo.TriggerGroupName)
                .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionFireAndProceed())
                .Build();
            _scheduler.RescheduleJob(triggerKey, trigger);
            return true;
        }

        /// <summary>
        /// 更改任务运行周期
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cron"></param>
        /// <returns></returns>
        public bool ModifyJobCron(int id, string cron)
        {
            var jobInfo = _jobDetailRepository.FirstOrDefault(x => x.Id == id);

            JobKey jobKey = CreateJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            if (_scheduler.CheckExists(jobKey).Result)
            {
                CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(cron);
                var triggerKey = CreateTriggerKey(jobInfo.TriggerName, jobInfo.TriggerGroupName);
                ITrigger trigger = TriggerBuilder.Create().StartNow()
                    .WithIdentity(jobInfo.TriggerName, jobInfo.TriggerGroupName)
                    .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionDoNothing())
                    .Build();
                _scheduler.RescheduleJob(triggerKey, trigger);
            }

            jobInfo.Cron = cron;
            _jobDetailRepository.Update(jobInfo);
            return true;
        }

        public bool ModifyExceptionEmail(int id, string exceptionEmail)
        {
            var jobInfo = _jobDetailRepository.FirstOrDefault(x => x.Id == id);
            jobInfo.ExceptionEmail = exceptionEmail;
            _jobDetailRepository.Update(jobInfo);
            return true;
        }

        public bool ModifyRequestUrl(int id, string requestUrl)
        {
            var jobInfo = _jobDetailRepository.FirstOrDefault(x => x.Id == id);
            jobInfo.RequestUrl = requestUrl;
            _jobDetailRepository.Update(jobInfo);
            return true;
        }

        public List<QuartzInstanceOutputDto> GetQuartzInstances()
        {
            var instanceInfos = _dbContext.QRTZ_SCHEDULER_STATE.AsNoTracking()
                .Where(x => x.SCHED_NAME == _quartzInstanceName)
                .ToList();
            var result = new List<QuartzInstanceOutputDto>();
            if (instanceInfos.Any())
            {
                foreach (var item in instanceInfos)
                {
                    DateTime tmDateUtc = new DateTime(item.LAST_CHECKIN_TIME, DateTimeKind.Utc);

                    DateTime tmDate = DateTime.SpecifyKind(tmDateUtc, DateTimeKind.Local).AddHours(8);

                    var heartBeat = DateDiff(DateTime.Now, tmDate);

                    result.Add(new QuartzInstanceOutputDto
                    {
                        InstanceName = item.INSTANCE_NAME,
                        LastCheckInTime = tmDate,
                        HeartBeat = heartBeat
                    });
                }
            }

            return result;
        }


        private JobKey CreateJobKey(string jobName, string jobGroupName)
        {
            return new JobKey(jobName, jobGroupName);

        }
        private TriggerKey CreateTriggerKey(string triggerName, string triggerGroupName)
        {
            return new TriggerKey(triggerName, triggerGroupName);
        }

        private string GetJobStatusByKey(TriggerKey triggerKey)
        {
            var status = _scheduler.GetTriggerState(triggerKey).Result;
            return status.ToString();
        }

        private string DateDiff(DateTime dateTime1, DateTime dateTime2)
        {
            TimeSpan ts1 = new TimeSpan(dateTime1.Ticks);
            TimeSpan ts2 = new
                TimeSpan(dateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();

            if (ts.Days > 0)
            {
                return ts.Days + "天前";
            }

            if (ts.Hours > 0)
            {
                return ts.Hours + "小时前";
            }

            if (ts.Minutes > 0)
            {
                return ts.Minutes + "分钟前";
            }

            if (ts.Seconds > 0)
            {
                return ts.Seconds + "秒前";
            }

            if (ts.Milliseconds > 0)
            {
                return "1秒内";
            }

            return string.Empty;
        }
    }
}
