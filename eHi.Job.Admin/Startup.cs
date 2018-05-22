using eHi.Job.Core;
using FeI.Web;
using Owin;

namespace eHi.Job.Admin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //设置启动模块
            app.UseFeI<EhiJobAdminModule>();
            var cockQuartzCoreModule = new EhiJobCoreModule();
        }
    }
}