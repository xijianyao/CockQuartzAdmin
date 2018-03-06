using FeI.Web;
using Owin;

namespace CockQuartzAdmin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //设置启动模块
            app.UseFeI<CockQuartzAdminModule>();
        }
    }
}