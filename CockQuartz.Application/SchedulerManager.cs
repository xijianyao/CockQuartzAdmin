using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using CockQuartz.Core.Infrastructure;
using Quartz;
using Quartz.Impl;

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

        public static IScheduler GetScheduler()
        {
            try
            {
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                var scheduler = schedulerFactory.GetScheduler(ApiJobSettings.QuartzInstanceName).Result;
                return scheduler;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}