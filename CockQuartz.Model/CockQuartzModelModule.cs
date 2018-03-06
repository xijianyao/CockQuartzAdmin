using System.Data.Entity;
using System.Reflection;
using FeI;
using FeI.Modules;
using Module = FeI.Modules.Module;

namespace CockQuartz.Model
{
    [DependsOn(typeof(EntityFrameworkModule))]
    public class CockQuartzModelModule : Module
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Database.SetInitializer<CockQuartzDbContext>(null);
        }
    }
}
