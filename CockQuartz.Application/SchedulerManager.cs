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

        //private static readonly string quartzScheduler_Address = ConfigurationManager.AppSettings["QuartzProxyAddress"];

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