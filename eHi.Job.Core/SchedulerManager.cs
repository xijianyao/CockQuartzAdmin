using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Threading.Tasks;
using eHi.Job.Core.Infrastructure;
using eHi.Job.Core.JobManager;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace eHi.Job.Core
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
                            _scheduler = GetScheduler().Result;
                        }
                    }
                }
                return _scheduler;
            }
        }

        private static async Task<IScheduler> GetScheduler()
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
                    { "quartz.jobStore.clusterCheckinInterval", "5000" },
                    { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                    { "quartz.serializer.type", "binary" },
                    { "quartz.jobStore.tablePrefix", "qrtz_" },
                    { "quartz.jobStore.driverDelegateType", "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz" },
                    { "quartz.jobStore.dataSource", "myDS" },
                    { "quartz.dataSource.myDS.connectionString", jobConnStr },
                    { "quartz.dataSource.myDS.provider", "SqlServer" },
                    { "quartz.scheduler.instanceId", "AUTO" }
                };
                var schedulerFactory = new StdSchedulerFactory(props);
                var scheduler = await schedulerFactory.GetScheduler();
                await scheduler.Clear();
                scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
                return scheduler;
            }
#pragma warning disable CS0168 // 声明了变量“e”，但从未使用过
            catch (Exception e)
#pragma warning restore CS0168 // 声明了变量“e”，但从未使用过
            {
                return null;
            }
        }
    }
}