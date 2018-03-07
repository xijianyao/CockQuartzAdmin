using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using CockQuartz.Interface;
using CockQuartz.Model;
using FeI.Application;
using FeI.Domain.Repositories;
using Quartz;
using Quartz.Impl.Matchers;

namespace CockQuartz.Application
{
    public class JobService : ApplicationService, IJobService
    {
        private readonly IScheduler _scheduler;
        private readonly IRepository<JobDetail> _jobDetailRepository;

        public JobService(IRepository<JobDetail> jobDetailRepository)
        {
            _jobDetailRepository = jobDetailRepository;
            _scheduler = SchedulerManager.Instance;
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
        public List<JobDetailOutputDto> GetJobList()
        {
            var jobList = _jobDetailRepository.Query().AsNoTracking()
                .Where(x => !x.IsDeleted).ToList();
            var result = new List<JobDetailOutputDto>();
            foreach (var groupName in _scheduler.GetJobGroupNames().Result)
            {
                foreach (JobKey jobKey in _scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(groupName)).Result)
                {
                    var triggers = _scheduler.GetTriggersOfJob(jobKey).Result.ToList();
                    foreach (var item in triggers)
                    {
                        var jobViewModel = new JobDetailOutputDto();
                        jobViewModel.JobName = jobKey.Name;
                        jobViewModel.JobGroupName = jobKey.Group;
                        jobViewModel.TriggerGroupName = item.Key.Group;
                        jobViewModel.TriggerName = item.Key.Name;
                        jobViewModel.CurrentStatus = GetJobStatusByKey(item.Key);
                        jobViewModel.NextRunTime = item.GetNextFireTimeUtc() == null
                            ? "下次不会触发运行时间"
                            : item.GetNextFireTimeUtc()?.LocalDateTime.ToString(CultureInfo.InvariantCulture);
                        jobViewModel.LastRunTime = item.GetPreviousFireTimeUtc() == null
                            ? "还未运行过"
                            : item.GetPreviousFireTimeUtc()?.LocalDateTime.ToString(CultureInfo.InvariantCulture);

                        var jobInfo = jobList.FirstOrDefault(x =>
                            x.JobName == jobKey.Name && x.JobGroupName == jobKey.Group);
                        jobViewModel.Cron = jobInfo.Cron;
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
                    result.Add(jobViewModel);
                }
            }

            return result;
        }

        /// <summary>
        /// 运行任务
        /// </summary>
        /// <param name="id">任务信息</param>
        /// <returns></returns>
        public bool RunJob(int id)
        {
            var jobInfo = _jobDetailRepository.FirstOrDefault(x => x.Id == id);

            JobKey jobKey = _createJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            if (!_scheduler.CheckExists(jobKey).Result)
            {
                IJobDetail job = JobBuilder.Create<JobBase>()
                    .WithIdentity(jobKey)
                    .UsingJobData(_createJobDataMap("jobId", jobInfo.Id))
                    .UsingJobData(_createJobDataMap("requestUrl", jobInfo.RequestUrl))//添加此任务请求地址附带到Context上下文中
                    .RequestRecovery(true)
                    .Build();

                CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(jobInfo.Cron);
                ITrigger trigger = TriggerBuilder.Create().StartNow()//StartAt(DateTime.SpecifyKind(jobInfo.JobStartTime, DateTimeKind.Local))
                    .WithIdentity(jobInfo.TriggerName, jobInfo.TriggerGroupName)
                    .ForJob(jobKey)
                    .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionDoNothing())
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

            var jobKey = _createJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            var triggerKey = _createTriggerKey(jobInfo.TriggerName, jobInfo.TriggerGroupName);
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
            var jobKey = _createJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            _scheduler.PauseJob(jobKey);
            return true;
        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        /// <param name="jobInfo">任务信息</param>
        /// <returns></returns>
        public bool ResumeJob(JobDetail jobInfo)
        {
            var jobKey = _createJobKey(jobInfo.JobName, jobInfo.JobGroupName);
            _scheduler.ResumeJob(jobKey);
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

            CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(jobInfo.Cron);
            var triggerKey = _createTriggerKey(jobInfo.TriggerName, jobInfo.TriggerGroupName);
            ITrigger trigger = TriggerBuilder.Create().StartNow()
                    .WithIdentity(jobInfo.TriggerName, jobInfo.TriggerGroupName)
                    .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionDoNothing())
                    .Build();
            _scheduler.RescheduleJob(triggerKey, trigger);
            jobInfo.Cron = cron;
            _jobDetailRepository.Update(jobInfo);
            return true;
        }

        /// <summary>
        /// 获取单个任务状态（从scheduler获取）
        /// </summary>
        /// <param name="triggerName"></param>
        /// <param name="triggerGroupName"></param>
        /// <returns></returns>
        private TriggerState _getTriggerState(string triggerName, string triggerGroupName)
        {

            TriggerKey triggerKey = _createTriggerKey(triggerName, triggerGroupName);
            var triggerState = _scheduler.GetTriggerState(triggerKey).Result;

            return triggerState;
        }

        private JobKey _createJobKey(string jobName, string jobGroupName)
        {
            return new JobKey(jobName, jobGroupName);

        }
        private TriggerKey _createTriggerKey(string triggerName, string triggerGroupName)
        {
            return new TriggerKey(triggerName, triggerGroupName);
        }
        private int _changeType(TriggerState triggerState)
        {
            switch (triggerState)
            {
                case TriggerState.None: return -1;
                case TriggerState.Normal: return 0;
                case TriggerState.Paused: return 1;
                case TriggerState.Complete: return 2;
                case TriggerState.Error: return 3;
                case TriggerState.Blocked: return 4;
                default: return -1;
            }

        }

        private JobDataMap _createJobDataMap<T>(string propertyName, T propertyValue)
        {
            return new JobDataMap(new Dictionary<string, T>() { { propertyName, propertyValue } });
        }

        private string GetJobStatusByKey(TriggerKey triggerKey)
        {
            var status = _scheduler.GetTriggerState(triggerKey).Result;
            return status.ToString();
        }
    }
}
