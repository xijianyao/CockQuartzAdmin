using System;
using System.Threading.Tasks;
using System.Web;
using Quartz;
using ServiceClients;

namespace CockQuartz.Application
{
    public class JobBase : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var serviceClient = new ServiceClient();
                await serviceClient.RequestAsync(context.JobDetail.JobDataMap["requestUrl"].ToString(), HttpVerb.Get);
            }
            catch (Exception ex)
            {
                
                throw new JobExecutionException(ex.Message);
            }

        }
    }
}
