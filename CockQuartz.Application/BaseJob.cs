using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CockQuartz.Model;
using FeI.Domain.Repositories;
using Quartz;
using ServiceClients;

namespace CockQuartz.Application
{
    public class JobBase : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var serviceClient = new ServiceClient();

            await serviceClient.RequestAsync(context.JobDetail.JobDataMap["requestUrl"].ToString(), HttpVerb.Get);
        }

    }
}
