using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace eHi.Job.Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static string Version { get; private set; }
        protected void Application_Start()
        {
        
            Version = Guid.NewGuid().ToString("N");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

    }
}
