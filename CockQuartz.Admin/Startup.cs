using CockQuartz.Core;
using FeI.Web;
using Owin;

namespace CockQuartz.Admin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //设置启动模块
            app.UseFeI<CockQuartzAdminModule>().Use<CockQuartzCoreModule>(null);
        }
    }
}