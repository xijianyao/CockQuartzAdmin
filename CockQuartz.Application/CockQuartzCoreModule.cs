using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using CockQuartz.Core.Infrastructure;
using CockQuartz.Model;
using eHi.Library.Common;
using eHi.Library.Integration.Common.Configuration;
using eHi.Library.Interface;
using eHi.Library.Service;
using FeI;
using FeI.Dependency;
using FeI.Domain.Uow;
using FeI.Modules;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Module = FeI.Modules.Module;

namespace CockQuartz.Core
{
    [DependsOn(typeof(EntityFrameworkModule), typeof(CockQuartzModelModule))]
    public class CockQuartzCoreModule : Module
    {
        public static readonly Dictionary<string, Type> _typeDic = new Dictionary<string, Type>();

        public override void Initialize()
        {
            IocManager.RegisterTypeIfNot<IConnectionStringResolver, ConnectionStringResolver>();
            IocManager.RegisterAssemblyByConvention(typeof(CockQuartzCoreModule).GetTypeInfo().Assembly);

            ConfigQuartz();
            ConfigJobMethods();
        }

        private void ConfigQuartz()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
            scheduler.Start();
        }

        private void ConfigJobMethods()
        {
            var platform = StartupConfig.CurrentPlatform;
            string assemblyFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + "CockQuartz.Jobs.dll";
            Assembly ass = Assembly.LoadFile(assemblyFilePath);
            var types = ass.GetTypes();

            var dbContext = DbContextFactory.DbContext;

            foreach (Type type in types)
            {
                if (!type.IsClass && !typeof(IJob).IsAssignableFrom(type))
                    continue;

                ApiJobAttribute member = type.GetCustomAttribute<ApiJobAttribute>(false);
                if (member != null)
                {
                    var apiJob = new JobDetail
                    {
                        JobName = member.Name,
                        JobGroupName = platform.ToString(),
                        TriggerGroupName = platform + member.Name + "TriggerGroup",
                        TriggerName = platform + member.Name + "Trigger",
                    };

                    var job = dbContext.JobDetail.FirstOrDefault(x => x.JobGroupName == apiJob.JobGroupName
                                                                      && x.JobName == apiJob.JobName
                                                                      && x.TriggerGroupName ==
                                                                      apiJob.TriggerGroupName
                                                                      && x.TriggerName == apiJob.TriggerName
                                                                      && !x.IsDeleted);
                    var jobInvocationType = new JobInvocationData
                    {
                        Type = type.ToString(),
                    };

                    if (!_typeDic.ContainsKey(type.ToString()))
                    {
                        _typeDic.Add(type.ToString(), type);
                    }

                    var jobInvocationData = JsonConvert.SerializeObject(jobInvocationType);
                    if (job != null)
                    {
                        job.CreateUser = apiJob.CreateUser;
                        job.ExceptionEmail = apiJob.ExceptionEmail;
                        job.Description = apiJob.Description;
                        job.UpdateTime = DateTime.Now;
                        job.UpdateUser = apiJob.CreateUser;
                        job.InvocationData = jobInvocationData;
                        dbContext.JobDetail.AddOrUpdate(job);
                        dbContext.SaveChanges();
                        ScheduleJob(job);
                    }
                    else
                    {
                        apiJob.CreateTime = DateTime.Now;
                        apiJob.InvocationData = jobInvocationData;
                        dbContext.JobDetail.AddOrUpdate(apiJob);
                        dbContext.SaveChanges();
                    }

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
            if (!scheduler.CheckExists(jobKey).Result)
            {
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
}
