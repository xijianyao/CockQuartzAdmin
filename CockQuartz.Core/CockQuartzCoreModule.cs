using System;
using System.Reflection;
using CockQuartz.Application;
using CockQuartz.Core.Infrastructure;
using CockQuartz.Model;
using eHi.Library.Integration.Common.Configuration;
using eHi.Library.Interface;
using eHi.Library.Service;
using FeI.Dependency;
using FeI.Domain.Uow;
using FeI.Modules;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Module = FeI.Modules.Module;

namespace CockQuartz.Core
{
    [DependsOn(typeof(CockQuartzModelModule))]
    public class CockQuartzCoreModule : Module
    {
        public override void PreInitialize()
        {
#if DEBUG
            Configuration.Modules.IntegrationModule().DisableDbConfig = true;
#endif
            //IocManager.RegisterTypeIfNot<IConnectionStringResolver, ConnectionStringResolver>();
        }

        public override void Initialize()
        {
            IocManager.RegisterTypeIfNot<IDbConnectionStringResolver, DefaultDbConnectionStringResolver>();
            IocManager.RegisterAssemblyByConvention(typeof(CockQuartzCoreModule).GetTypeInfo().Assembly);
            ConfigQuartz();
        }

        private void ConfigQuartz()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.ListenerManager.AddJobListener(new JobListener(), GroupMatcher<JobKey>.AnyGroup());
            scheduler.Start();
        }
    }
}
