using System;
using Quartz.Impl;
using System.Collections.Specialized;
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
            var properties = PropertiesConfig();
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
            var scheduler = schedulerFactory.GetScheduler().Result;
            //scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
            scheduler.Start();
            Console.WriteLine(scheduler.SchedulerInstanceId);
            Console.WriteLine(scheduler.SchedulerName);

            var url = "http://+:8080";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Running on {0}", url);
                Console.WriteLine("Press enter to exit");
            }
            Console.ReadLine();
        }

        private static NameValueCollection PropertiesConfig()
        {
            var properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = "RemoteServer",//名称
                ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                ["quartz.threadPool.threadCount"] = "5",
                ["quartz.threadPool.threadPriority"] = "Normal",
                ["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz", //集群配置
                ["quartz.scheduler.exporter.port"] = "555",//端口号
                ["quartz.scheduler.exporter.bindName"] = "QuartzScheduler1",
                ["quartz.scheduler.exporter.channelType"] = "tcp",//通道类型
                ["quartz.scheduler.exporter.channelName"] = "httpQuartz",
                ["quartz.scheduler.exporter.rejectRemoteRequests"] = "true",
                ["quartz.jobStore.clustered"] = "true",
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",//存储类型
                ["quartz.serializer.type"] = "json",
                ["quartz.jobStore.tablePrefix"] = "qrtz_",//表名前缀
                ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz",//驱动类型
                ["quartz.jobStore.dataSource"] = "myDS",//数据源名称
                ["quartz.dataSource.myDS.connectionString"] =
                    ConfigurationManager.ConnectionStrings["CockQuartz"].ToString(),//连接字符串
                ["quartz.dataSource.myDS.provider"] = "SqlServer",   //版本
                ["quartz.scheduler.instanceId"] = "AUTO"
            };
            return properties;
        }
    }
}
