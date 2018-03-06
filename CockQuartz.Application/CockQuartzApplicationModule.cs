using System.Reflection;
using CockQuartz.Model;
using FeI;
using FeI.Modules;
using Module = FeI.Modules.Module;

namespace CockQuartz.Application
{
    [DependsOn(typeof(EntityFrameworkModule),typeof(CockQuartzModelModule))]
    public class CockQuartzApplicationModule : Module
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
