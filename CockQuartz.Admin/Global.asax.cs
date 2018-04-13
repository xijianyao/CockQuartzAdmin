using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CockQuartzAdmin.JobHandler;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace CockQuartzAdmin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

    }
}
