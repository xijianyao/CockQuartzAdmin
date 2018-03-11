using System;
using Quartz.Impl;
using System.Configuration;
using Microsoft.Owin.Hosting;
using Quartz;
using Quartz.Impl.Matchers;

namespace CockQuartz.RemoteServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
            scheduler.Start();
            Console.WriteLine(scheduler.SchedulerInstanceId);
            Console.WriteLine(scheduler.SchedulerName);

            var url = ConfigurationManager.AppSettings["owinUrl"];
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Running on {0}", url);
                Console.WriteLine("Press enter to exit");
            }
            Console.ReadLine();
        }
    }
}
