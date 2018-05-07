using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Reflection;
using CockQuartz.Application.Infrastructure;
using CockQuartz.Model;
using eHi.Library.Common;
using eHi.Library.Integration.Common.Configuration;
using eHi.Library.Interface;
using eHi.Library.Service;
using FeI;
using FeI.Configuration;
using FeI.Dependency;
using FeI.Domain.Uow;
using FeI.Modules;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Module = FeI.Modules.Module;

namespace CockQuartz.Application
{
    [DependsOn(typeof(EntityFrameworkModule), typeof(CockQuartzModelModule))]
    public class CockQuartzApplicationModule : Module
    {
        public static readonly Dictionary<string, MethodInfo> _typeDic = new Dictionary<string, MethodInfo>();

        public override void PreInitialize()
        {
#if DEBUG
            Configuration.Modules.IntegrationModule().DisableDbConfig = true;
#endif
            IocManager.RegisterTypeIfNot<IConnectionStringResolver, ConnectionStringResolver>();
        }

        public override void Initialize()
        {
            IocManager.RegisterTypeIfNot<IDbConnectionStringResolver, DefaultDbConnectionStringResolver>();
            IocManager.RegisterAssemblyByConvention(typeof(CockQuartzApplicationModule).GetTypeInfo().Assembly);
            
            ConfigQuartz();
        }

        public override void PostInitialize()
        {
            ConfigJobMethods();
        }

        private void ConfigQuartz()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
            //scheduler.JobFactory = IocManager.Resolve<IJobFactory>();
            scheduler.Start();
        }

        private void ConfigJobMethods()
        {
            //IocManager.RegisterTypeIfNot<IJobFactory, JobService.IsolatedJobFactory>();
            var platform = StartupConfig.CurrentPlatform;
            string assemblyFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + "CockQuartz.Admin.dll";
            Assembly ass = Assembly.LoadFile(assemblyFilePath);
            var types = ass.GetTypes();

            var dbContext = DbContextFactory.DbContext;

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
                    var methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.OptionalParamBinding);
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
                                    JobName = member.Name,
                                    JobGroupName = platform.ToString(),
                                    TriggerGroupName = platform + member.Name + "TriggerGroup",
                                    TriggerName = platform + member.Name + "Trigger",
                                    CreateUser = member.Developer,
                                    ExceptionEmail = member.DeveloperMail,
                                    Description = member.Description
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
                                    Method = methodInfo.Name
                                };

                                if (!_typeDic.ContainsKey(type.ToString()))
                                {
                                    _typeDic.Add(type.ToString(), methodInfo);
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
                                    ScheduleJob(job);
                                }
                                else
                                {
                                    apiJob.CreateTime = DateTime.Now;
                                    apiJob.InvocationData = jobInvocationData;
                                    dbContext.JobDetail.AddOrUpdate(apiJob);
                                }

                            }
                        }
                    }
                }
            }

            dbContext.SaveChanges();
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
