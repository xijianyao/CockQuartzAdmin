using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using CockQuartz.Core.Infrastructure;
using CockQuartz.Core.JobManager;
using CockQuartz.Model;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Module = FeI.Modules.Module;

namespace CockQuartz.Core
{
    public class CockQuartzCoreModule : Module
    {
        public static readonly Dictionary<int, MethodInfo> MethodsDic = new Dictionary<int, MethodInfo>();
        private readonly JobMangerDal _jobMangerDal;

        public CockQuartzCoreModule()
        {
            _jobMangerDal = new JobMangerDal();
            ConfigQuartz();
            ConfigJobs();
        }

        public override void Initialize()
        {

        }

        private void ConfigQuartz()
        {
            var jobConnStr = JobMangerDal.GetConString();
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.scheduler.instanceName", ApiJobSettings.QuartzInstanceName },
                { "quartz.threadPool.type", "Quartz.Simpl.SimpleThreadPool, Quartz" },
                { "quartz.threadPool.threadCount", "20" },
                { "quartz.threadPool.threadPriority", "Normal" },
                { "quartz.jobStore.clustered", "true" },
                { "quartz.jobStore.clusterCheckinInterval", "1000" },
                { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.serializer.type", "json" },
                { "quartz.jobStore.tablePrefix", "qrtz_" },
                { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz" },
                { "quartz.jobStore.dataSource", "myDS" },
                { "quartz.dataSource.myDS.connectionString", jobConnStr },
                { "quartz.dataSource.myDS.provider", "SqlServer" },
                { "quartz.scheduler.instanceId", "AUTO" }
            };
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(props);
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
            scheduler.Start();
        }

        private void ConfigJobs()
        {
            string assemblyFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + ApiJobSettings.ApiJobAssemblyName;
            Assembly ass = Assembly.LoadFile(assemblyFilePath);
            var types = ass.GetTypes();

            var jobList = _jobMangerDal.GetJobDetailsByGroupName(ApiJobSettings.ApiJobSystemName);

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
                                    JobName = member.Name,
                                    JobGroupName = ApiJobSettings.ApiJobSystemName,
                                    TriggerGroupName = ApiJobSettings.ApiJobSystemName + member.Name + "TriggerGroup",
                                    TriggerName = ApiJobSettings.ApiJobSystemName + member.Name + "Trigger",
                                    CreateUser = member.ApiJobDeveloper
                                };

                                var job = jobList.FirstOrDefault(x => x.JobName == apiJob.JobName
                                                                      && x.TriggerGroupName == apiJob.TriggerGroupName
                                                                      && x.TriggerName == apiJob.TriggerName);
                                var jobInvocationType = new JobInvocationData
                                {
                                    Type = type.ToString(),
                                    Method = methodInfo.ToString()
                                };
                                var jobInvocationData = JsonConvert.SerializeObject(jobInvocationType);
                                int jobId;
                                if (job != null)
                                {
                                    job.CreateUser = apiJob.CreateUser;
                                    job.ExceptionEmail = apiJob.ExceptionEmail;
                                    job.Description = apiJob.Description;
                                    job.UpdateTime = DateTime.Now;
                                    job.UpdateUser = apiJob.CreateUser;
                                    job.InvocationData = jobInvocationData;
                                    _jobMangerDal.UpdateJobDetail(job);
                                    ScheduleJob(job);
                                    jobId = job.Id;
                                }
                                else
                                {
                                    apiJob.CreateTime = DateTime.Now;
                                    apiJob.InvocationData = jobInvocationData;
                                    jobId = _jobMangerDal.InsertJobDetailAndGetId(apiJob);
                                }

                                if (!MethodsDic.ContainsKey(jobId))
                                {
                                    MethodsDic.Add(jobId, methodInfo);
                                }

                            }
                        }
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
