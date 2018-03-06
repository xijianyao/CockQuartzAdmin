using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using CockQuartz.Application;
using eHi.Library.Integration.Admin;
using FeI.Dependency;
using FeI.Modules;
using Module = FeI.Modules.Module;

namespace CockQuartzAdmin
{
    [DependsOn(typeof(AdminModule), typeof(CockQuartzApplicationModule))]
    public class CockQuartzAdminModule : Module
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AdminModule).GetTypeInfo().Assembly);
        }
    }
}