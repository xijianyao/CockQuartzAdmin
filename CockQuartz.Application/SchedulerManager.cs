using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Configuration;
using Quartz;
using Quartz.Impl;

namespace CockQuartz.Application
{
    public class SchedulerManager
    {
        static readonly object Locker = new object();
        static IScheduler _scheduler;
        public static readonly ConcurrentDictionary<string, IScheduler> ConnectionCache = new ConcurrentDictionary<string, IScheduler>();

        private static readonly string quartzScheduler1_Address = ConfigurationManager.AppSettings["QuartzProxyAddress1"];
        private static readonly string quartzScheduler2_Address = ConfigurationManager.AppSettings["QuartzProxyAddress1"];

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

        public static IScheduler GetScheduler()
        {
            if (!ConnectionCache.ContainsKey(quartzScheduler1_Address))
            {
                var properties = new NameValueCollection();
                properties["quartz.scheduler.proxy"] = "true";
                properties["quartz.scheduler.proxy.address"] = quartzScheduler1_Address;
                var schedulerFactory = new StdSchedulerFactory(properties);
                _scheduler = schedulerFactory.GetScheduler().Result;
                ConnectionCache[quartzScheduler1_Address] = _scheduler;
            }
            if (!ConnectionCache.ContainsKey(quartzScheduler2_Address))
            {
                var properties = new NameValueCollection();
                properties["quartz.scheduler.proxy"] = "true";
                properties["quartz.scheduler.proxy.address"] = quartzScheduler2_Address;
                var schedulerFactory = new StdSchedulerFactory(properties);
                _scheduler = schedulerFactory.GetScheduler().Result;
                ConnectionCache[quartzScheduler2_Address] = _scheduler;
            }

            return ConnectionCache[quartzScheduler1_Address];
        }

    }
}