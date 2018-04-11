using System.Net;
using System.Net.Http;
using System.Reflection;
using eHi.Library.Integration.Common;
using eHi.Library.Integration.Common.Configuration;
using FeI;
using FeI.Modules;
using ServiceClients;
using Module = FeI.Modules.Module;

namespace CockQuartz.RemoteServer
{
    [DependsOn(typeof(EntityFrameworkModule), typeof(IntegrationCommonModule))]
    public class CockQuartzRemoteServerModule : Module
    {
        public override void PreInitialize()
        {
            ConfigServiceClient();
            Configuration.Modules.IntegrationModule().ServiceClientConfig.IsAttachData = false;
        }


        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        private void ConfigServiceClient()
        {
            if (!IocManager.IsRegistered<IServiceClient>())
            {
                var serviceClient =
                    new ServiceClient(new WorkerOperationIdHttpHandler
                    {
                        InnerHandler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip }
                    }
                    );
                IocManager.RegisterInstance<IServiceClient>(serviceClient);

            }
        }
    }
}
