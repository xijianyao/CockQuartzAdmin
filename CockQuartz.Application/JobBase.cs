using System;
using System.Reflection;
using System.Threading.Tasks;
using CockQuartz.Application.Infrastructure;
using Newtonsoft.Json;
using Quartz;
using ServiceClients;

namespace CockQuartz.Application
{
    public class JobBase : IJob
    {
        private readonly IServiceClient _serviceClient;

        public JobBase(IServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var type = JsonConvert.DeserializeObject<JobInvocationData>(context.JobDetail.JobDataMap["invocationData"].ToString());

                var method = CockQuartzApplicationModule._typeDic[type.Type];

                object instance = Activator.CreateInstance(method.DeclaringType);
                method.Invoke(instance, BindingFlags.OptionalParamBinding | BindingFlags.InvokeMethod, null, null, System.Globalization.CultureInfo.CurrentCulture);

                //await _serviceClient.RequestAsync(context.JobDetail.JobDataMap["invocationData"].ToString(), HttpVerb.Get);
            }
            catch (Exception ex)
            {

                throw new JobExecutionException(ex.Message);
            }
        }
    }
}
