using Nancy;

namespace CockQuartz.RemoteServer
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = x => "Started";
        }
    }
}
