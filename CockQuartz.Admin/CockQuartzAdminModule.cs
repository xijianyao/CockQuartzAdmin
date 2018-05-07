using System;
using System.Reflection;
using CockQuartz.Application;
using eHi.Common.Enum;
using eHi.Library.Extensions;
using eHi.Library.Integration.Admin;
using eHi.Library.Integration.Api;
using eHi.Library.Integration.Common.Configuration;
using eHi.Library.Interface;
using eHi.Library.Service;
using FeI.Dependency;
using FeI.Modules;
using Module = FeI.Modules.Module;

namespace CockQuartz.Admin
{
    [DependsOn(typeof(AdminModule), typeof(CockQuartzApplicationModule), typeof(ApiModule))]
    public class CockQuartzAdminModule : Module
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
            IocManager.RegisterAssemblyByConvention(typeof(CockQuartzAdminModule).GetTypeInfo().Assembly);
            Configuration
                .SetPlatform(Platform.Test);
        }

    }
}