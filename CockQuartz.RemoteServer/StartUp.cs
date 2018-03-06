using FeI.Web;
using Owin;

namespace CockQuartz.RemoteServer
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseNancy().UseFeI<CockQuartzRemoteServerModule>();
        }
    }
}
