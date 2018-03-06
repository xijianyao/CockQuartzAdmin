using System.Reflection;
using CockQuartz.Application;
using FeI;
using FeI.Dependency;
using FeI.EntityFramework;
using FeI.Modules;
using Module = FeI.Modules.Module;

namespace CockQuartz.RemoteServer
{
    [DependsOn(typeof(CockQuartzApplicationModule), typeof(EntityFrameworkModule))]
    public class CockQuartzRemoteServerModule : Module
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());            
        }
    }
}
