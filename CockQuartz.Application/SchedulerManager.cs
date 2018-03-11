using System;
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

        private static readonly string quartzScheduler_Address = ConfigurationManager.AppSettings["QuartzProxyAddress"];

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
            try
            {
                if (!ConnectionCache.ContainsKey(quartzScheduler_Address))
                {
                    var properties = new NameValueCollection();
                    properties["quartz.scheduler.proxy"] = "true";
                    properties["quartz.scheduler.proxy.address"] = quartzScheduler_Address;
                    var schedulerFactory = new StdSchedulerFactory(properties);
                    _scheduler = schedulerFactory.GetScheduler().Result;
                    ConnectionCache[quartzScheduler_Address] = _scheduler;
                }

                return ConnectionCache[quartzScheduler_Address];
            }
            catch (Exception e)
            {
                return null;
            }
            
        }

    }
}