using System;
using System.Collections.Concurrent;
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
                return schedulerFactory.GetScheduler().Result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}