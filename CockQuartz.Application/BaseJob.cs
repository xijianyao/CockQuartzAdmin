using System;
using System.Threading.Tasks;
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
                await _serviceClient.RequestAsync(context.JobDetail.JobDataMap["requestUrl"].ToString(), HttpVerb.Get);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

    }
}
