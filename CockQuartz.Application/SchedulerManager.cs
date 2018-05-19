using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using CockQuartz.Core.Infrastructure;
using CockQuartz.Core.JobManager;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace CockQuartz.Core
{
    public class SchedulerManager
    {
        static readonly object Locker = new object();
        static IScheduler _scheduler;
        public static readonly ConcurrentDictionary<string, IScheduler> ConnectionCache = new ConcurrentDictionary<string, IScheduler>();

        public static IScheduler Instance
        {
            get
            {
                if (_scheduler == null)
                {
                    lock (Locker)
                    {
                        if (_scheduler == null)
                        {
                            _scheduler = GetScheduler();
                        }
                    }
                }
                return _scheduler;
            }
        }

        private static IScheduler GetScheduler()
        {
            try
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
                    { "quartz.serializer.type", "binary" },
                    { "quartz.jobStore.tablePrefix", "qrtz_" },
                    { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz" },
                    { "quartz.jobStore.dataSource", "myDS" },
                    { "quartz.dataSource.myDS.connectionString", jobConnStr },
                    { "quartz.dataSource.myDS.provider", "SqlServer" },
                    { "quartz.scheduler.instanceId", "AUTO" }
                };
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory(props);
                //ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                var scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
                //scheduler.Start();
                return scheduler;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}