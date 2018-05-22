using System.Reflection;
using eHi.Common.Enum;
using eHi.Job.Core;
using eHi.Library.Extensions;
using eHi.Library.Integration.Admin;
using eHi.Library.Integration.Api;
using eHi.Library.Integration.Common.Configuration;
using eHi.Library.Interface;
using eHi.Library.Service;
using FeI.Dependency;
using FeI.Modules;
using Module = FeI.Modules.Module;

namespace eHi.Job.Admin
{
    [DependsOn(typeof(AdminModule), typeof(EhiJobCoreModule), typeof(ApiModule))]
    public class EhiJobAdminModule : Module
    {
        public override void PreInitialize()
        {
#if DEBUG
            Configuration.Modules.IntegrationModule().DisableDbConfig = true;
#endif
            //IocManager.RegisterTypeIfNot<IConnectionStringResolver, ConnectionStringResolver>();
            Configuration
                .SetPlatform(Platform.Test);
        }

        public override void Initialize()
        {
            IocManager.RegisterTypeIfNot<IDbConnectionStringResolver, DefaultDbConnectionStringResolver>();
            IocManager.RegisterAssemblyByConvention(typeof(EhiJobAdminModule).GetTypeInfo().Assembly);
        }

    }
}