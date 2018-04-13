using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CockQuartz.Application;
using CockQuartz.Model;

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
            var jobExecuteLogs = new JobExecuteLogs();
            var exceptionEmail = string.Empty;
            var jobName = string.Empty;
            var a = context.FireInstanceId;
            var b = context.JobInstance;
            try
            {
                var jobId = int.Parse(context.JobDetail.JobDataMap["jobId"].ToString());
                var jobDetail = _dbContext.JobDetail.AsNoTracking()
                    .FirstOrDefault(x => x.Id == jobId);

                if (jobDetail == null)
                {
                    throw new JobExecutionException("数据库中未找到该job");
                }

                if (string.IsNullOrWhiteSpace(jobDetail.RequestUrl))
                {
                    throw new JobExecutionException("requestUrl为空");
                }

                context.JobDetail.JobDataMap.Put("requestUrl", jobDetail.RequestUrl);
                context.JobDetail.JobDataMap.Put("jobName", jobDetail.JobName);
                context.JobDetail.JobDataMap.Put("exceptionEmail", jobDetail.ExceptionEmail);
                context.JobDetail.JobDataMap.Put("jobGroupName", jobDetail.JobGroupName);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                _Stopwatches.Add(context.FireInstanceId, stopwatch);

                jobExecuteLogs.JobDetailId = jobDetail.Id;
                jobExecuteLogs.JobName = jobDetail.JobName;
                jobExecuteLogs.JobGroupName = jobDetail.JobGroupName;
                jobExecuteLogs.Message = $"JobName:{jobDetail.JobName},开始执行...";
                jobExecuteLogs.IsSuccess = true;

                exceptionEmail = jobDetail.ExceptionEmail;
            }
            catch (Exception ex)
            {
                jobExecuteLogs.Message = $"JobName:{jobName},执行出现异常:{ex.Message}";
                jobExecuteLogs.IsSuccess = false;
                jobExecuteLogs.ExceptionMessage = ex.Message;
                jobExecuteLogs.ExceptionStack = ex.StackTrace;
            }

            _dbContext.JobExecuteLogs.Add(jobExecuteLogs);
            if (!jobExecuteLogs.IsSuccess && !string.IsNullOrWhiteSpace(exceptionEmail))
            {
                SendExceptionEmail(exceptionEmail, jobExecuteLogs.JobName, jobExecuteLogs.ExceptionMessage, jobExecuteLogs.ExceptionStack);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {

            var jobExecuteLogs =
                new JobExecuteLogs
                {
                    JobDetailId = int.Parse(context.JobDetail.JobDataMap["jobId"].ToString()),
                    JobName = context.JobDetail.JobDataMap["jobName"].ToString(),
                    JobGroupName = context.JobDetail.JobDataMap["jobGroupName"].ToString()
                };

            var exceptionEmail = string.Empty;

            try
            {
                if (jobException != null)
                {
                    throw jobException;
                }

                var elapsed = _Stopwatches[context.FireInstanceId].ElapsedMilliseconds;


                jobExecuteLogs.Message = $"JobName:{context.JobDetail.JobDataMap["jobName"]},执行结束。执行时间:{elapsed}ms";
                jobExecuteLogs.IsSuccess = true;
            }
            catch (Exception ex)
            {
                jobExecuteLogs.Message = $"JobName:{context.JobDetail.JobDataMap["jobName"]},执行出现异常:{ex.Message}";
                jobExecuteLogs.IsSuccess = false;
                jobExecuteLogs.ExceptionMessage = ex.Message;
                jobExecuteLogs.ExceptionStack = ex.StackTrace;
            }

            _dbContext.JobExecuteLogs.Add(jobExecuteLogs);
            if (!jobExecuteLogs.IsSuccess && !string.IsNullOrWhiteSpace(exceptionEmail))
            {
                SendExceptionEmail(exceptionEmail, jobExecuteLogs.JobName, jobExecuteLogs.ExceptionMessage, jobExecuteLogs.ExceptionStack);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private void SendExceptionEmail(string email, string jobName, string exceptionMessage, string exceptionStack)
        {

        }
    }
}
