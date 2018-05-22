﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using eHi.Job.Core.Infrastructure;
using eHi.Job.Core.JobManager;
using eHi.Job.Core.Models;
using Quartz;
using Module = FeI.Modules.Module;

namespace eHi.Job.Core
{
    public class EhiJobCoreModule : Module
    {
        public static readonly Dictionary<int, MethodInfo> MethodsDic = new Dictionary<int, MethodInfo>();
        private readonly JobMangerDal _jobMangerDal;

        public EhiJobCoreModule()
        {
            if (ApiJobSettings.ApiJobSchedulerEnabled)
            {
                _jobMangerDal = new JobMangerDal();

                var scheduler = SchedulerManager.Instance;
                ConfigJobs();
                scheduler.Start();
            }
        }

        public override void Initialize()
        {

        }

        private void ConfigJobs()
        {
            string assemblyFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + ApiJobSettings.ApiJobAssemblyName;
            Assembly ass = Assembly.LoadFile(assemblyFilePath);
            var types = ass.GetTypes();

            var jobList = _jobMangerDal.GetJobDetailsByGroupName(ApiJobSettings.ApiJobSystemName);
            var newJobIdList = new List<int>();

            foreach (Type type in types)
            {
                if (!type.IsClass)
                    continue;

                bool mark = false;
                var tempType = type.BaseType;
                while (tempType != null)
                {
                    if (tempType.FullName == "System.Web.Http.ApiController")
                    {
                        mark = true;
                        break;
                    }
                    tempType = tempType.BaseType;
                }

                if (mark)
                {
                    var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance |
                                                  BindingFlags.Public | BindingFlags.OptionalParamBinding);
                    if (methods.Length == 0)
                    {
                        continue;
                    }

                    foreach (MethodInfo methodInfo in methods)
                    {
                        if (methodInfo.ReturnType.Name == "Void" && !methodInfo.GetParameters().Any())
                        {
                            ApiJobAttribute member = methodInfo.GetCustomAttribute<ApiJobAttribute>(false);
                            if (member != null)
                            {
                                var apiJob = new JobDetail
                                {
                                    JobName = member.ApiJobName,
                                    JobGroupName = ApiJobSettings.ApiJobSystemName,
                                    TriggerGroupName = ApiJobSettings.ApiJobSystemName + member.ApiJobName + "TriggerGroup",
                                    TriggerName = ApiJobSettings.ApiJobSystemName + member.ApiJobName + "Trigger",
                                    CreateUser = member.ApiJobDeveloper,
                                    ExceptionEmail = member.ApiJobDeveloperMail,
                                    Description = member.ApiJobDescription
                                };

                                var job = jobList.FirstOrDefault(x => x.JobName == apiJob.JobName
                                                                      && x.TriggerGroupName == apiJob.TriggerGroupName
                                                                      && x.TriggerName == apiJob.TriggerName);
                                int jobId;
                                if (job != null)
                                {
                                    job.CreateUser = apiJob.CreateUser;
                                    job.ExceptionEmail = apiJob.ExceptionEmail;
                                    job.Description = apiJob.Description;
                                    job.UpdateTime = DateTime.Now;
                                    job.UpdateUser = apiJob.CreateUser;
                                    _jobMangerDal.UpdateJobDetail(job);
                                    ScheduleJob(job);
                                    jobId = job.Id;
                                }
                                else
                                {
                                    apiJob.CreateTime = DateTime.Now;
                                    jobId = _jobMangerDal.InsertJobDetailAndGetId(apiJob);
                                }

                                if (!MethodsDic.ContainsKey(jobId))
                                {
                                    MethodsDic.Add(jobId, methodInfo);
                                }

                                newJobIdList.Add(jobId);
                            }
                        }
                    }
                }
            }

            foreach (var job in jobList)
            {
                if (newJobIdList.All(x => x != job.Id))
                {
                    _jobMangerDal.DeleteJobDetail(job.Id);
                }
            }
        }

        private void ScheduleJob(JobDetail jobDetail)
        {
            if (string.IsNullOrWhiteSpace(jobDetail.Cron))
            {
                return;
            }

            var scheduler = SchedulerManager.Instance;
            JobKey jobKey = new JobKey(jobDetail.JobName, jobDetail.JobGroupName);

            IJobDetail job = JobBuilder.Create<JobBase>()
                .WithIdentity(jobKey)
                .WithDescription(jobDetail.Description)
                .Build();

            JobDataMap map = job.JobDataMap;
            map.Put("jobId", jobDetail.Id);

            CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(jobDetail.Cron);
            ITrigger trigger = TriggerBuilder.Create().StartNow()//StartAt(DateTime.SpecifyKind(jobInfo.JobStartTime, DateTimeKind.Local))
                .WithIdentity(jobDetail.TriggerName, jobDetail.TriggerGroupName)
                .ForJob(jobKey)
                .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionDoNothing())
                .WithDescription(jobDetail.Description)
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}