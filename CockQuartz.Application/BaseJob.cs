using System;
using System.Net.Http;
using System.Threading.Tasks;
using Quartz;

namespace CockQuartz.Application
{
    public class JobBase : IJob
    {
        //private readonly IServiceClient _serviceClient;

        //public JobBase(IServiceClient serviceClient)
        //{
        //    _serviceClient = serviceClient;
        //}

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine($"job begin:{context.JobDetail.JobDataMap["requestUrl"]}");
                //await _serviceClient.RequestAsync(context.JobDetail.JobDataMap["requestUrl"].ToString(), HttpVerb.Get);
                HttpClient hc = new HttpClient();
                await hc.GetAsync(context.JobDetail.JobDataMap["requestUrl"].ToString());
                Console.WriteLine($"job end:{context.JobDetail.JobDataMap["requestUrl"]}");
                //context.JobDetail.JobDataMap["message1"] = $"job begin:{context.JobDetail.JobDataMap["requestUrl"]}";
                //Console.WriteLine($"job begin:{context.JobDetail.JobDataMap["requestUrl"]}");
                //await _serviceClient.RequestAsync(context.JobDetail.JobDataMap["requestUrl"].ToString(), HttpVerb.Get);
                //context.JobDetail.JobDataMap["message1"] = $"job end:{context.JobDetail.JobDataMap["requestUrl"]}";
                //Random r = new Random();
                //int n = r.Next(0, 9999);
                //System.IO.File.WriteAllText($"F://test//{n}.txt", n.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"job Exception:{ex.Message}");
            }
        }

    }
}
