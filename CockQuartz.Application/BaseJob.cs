using System;
using System.Threading.Tasks;
using Quartz;
using ServiceClients;

namespace CockQuartz.Application
{
    public class JobBase : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (var serviceClient = new ServiceClient())
            {
                await serviceClient.RequestAsync(context.JobDetail.JobDataMap["requestUrl"].ToString(), HttpVerb.Get);
            }
        }
    }
}
