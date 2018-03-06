﻿using System;
using Quartz.Impl;
using System.Collections.Specialized;
using System.Configuration;
using Microsoft.Owin.Hosting;
using Quartz;

namespace CockQuartz.RemoteServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "RemoteServer";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "5";
            properties["quartz.threadPool.threadPriority"] = "Normal";
            properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
            properties["quartz.scheduler.exporter.port"] = "555";//端口号
            properties["quartz.scheduler.exporter.bindName"] = "QuartzScheduler";//名称
            //通道类型
            properties["quartz.scheduler.exporter.channelType"] = "tcp";
            properties["quartz.scheduler.exporter.channelName"] = "httpQuartz";
            properties["quartz.scheduler.exporter.rejectRemoteRequests"] = "true";
            //集群配置
            properties["quartz.jobStore.clustered"] = "true";
            //存储类型
            properties["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            properties["quartz.serializer.type"] = "json";
            //表名前缀
            properties["quartz.jobStore.tablePrefix"] = "qrtz_";
            //驱动类型
            properties["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";
            //数据源名称
            properties["quartz.jobStore.dataSource"] = "myDS";
            //连接字符串
            properties["quartz.dataSource.myDS.connectionString"] = ConfigurationManager.ConnectionStrings["quartz_analyticsEntities"].ToString();
            //版本
            properties["quartz.dataSource.myDS.provider"] = "SqlServer";
            properties["quartz.scheduler.instanceId"] = "AUTO";
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);
            var scheduler = schedulerFactory.GetScheduler().Result;
            //scheduler.ListenerManager.AddJobListener(new MyJobListener(), GroupMatcher<JobKey>.AnyGroup());
            scheduler.Start();

            var url = "http://+:8080";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Running on {0}", url);
                Console.WriteLine("Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
