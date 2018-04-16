using System;
using System.Reflection;
using CockQuartz.Application;
using CockQuartzAdmin.Infrastructure;
using CockQuartzAdmin.JobHandler;
using eHi.Library.Integration.Admin;
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

namespace CockQuartzAdmin
{
    [DependsOn(typeof(AdminModule), typeof(CockQuartzApplicationModule))]
    public class CockQuartzAdminModule : Module
    {
        public override void PreInitialize()
        {
#if DEBUG
            Configuration.Modules.IntegrationModule().DisableDbConfig = true;
#endif
            IocManager.RegisterTypeIfNot<IConnectionStringResolver, ConnectionStringResolver>();
        }

        public override void Initialize()
        {
            IocManager.RegisterTypeIfNot<IDbConnectionStringResolver, DefaultDbConnectionStringResolver>();
            IocManager.RegisterAssemblyByConvention(typeof(CockQuartzAdminModule).GetTypeInfo().Assembly);
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