using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CockQuartz.Application;
using CockQuartz.Model;
using CockQuartz.Model.Repository;
using FeI.Domain.Repositories;

namespace CockQuartz.RemoteServer
{
    public class JobListener : IJobListener
    {
        public string Name => "customerJobListener";
        private Dictionary<string, Stopwatch> _Stopwatches = new Dictionary<string, Stopwatch>();
        private readonly CockQuartzDbContext _dbContext = DbContextFactory.DbContext;

        public JobListener()
        {
        }

        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.CompletedTask;
        }

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var jobId = int.Parse(context.JobDetail.JobDataMap["jobId"].ToString());
                var jobDetail = _dbContext.JobDetail.AsNoTracking()
                    .FirstOrDefault(x => x.Id == jobId);

                if (jobDetail == null)
                {
                    throw new JobExecutionException("数据库中未找到job");
                }

                if (string.IsNullOrWhiteSpace(jobDetail.RequestUrl))
                {
                    throw new JobExecutionException("requestUrl为空");
                }

                context.JobDetail.JobDataMap.Put("requestUrl", jobDetail.RequestUrl);
                context.JobDetail.JobDataMap.Put("jobName", jobDetail.JobName);
                context.JobDetail.JobDataMap.Put("exceptionEmail", jobDetail.ExceptionEmail);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                Console.WriteLine("Job {0} in group {1} is about to be executed", context.JobDetail.Key.Name, context.JobDetail.Key.Group);
                _Stopwatches.Add(context.FireInstanceId, stopwatch);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息：{0}", ex.Message);

            }
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var elapsed = _Stopwatches[context.FireInstanceId].ElapsedMilliseconds;
                Console.WriteLine($"Job {context.JobDetail.Key.Name} in group {context.JobDetail.Key.Group} was executed " +
                                  $"in {elapsed}ms;{context.JobDetail.JobDataMap["requestUrl"]},{context.JobDetail.JobDataMap["jobName"]}," +
                                  $"{context.JobDetail.JobDataMap["exceptionEmail"]}");
                await Task.CompletedTask;
            }
            catch (JobExecutionException e)
            {
                Console.WriteLine("异常{0}", e.Message);
            }
        }
    }
}
