using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CockQuartz.Core.JobManager;
using CockQuartz.Model;
using Quartz;

namespace CockQuartz.Core.Infrastructure
{
    public class JobListener : IJobListener
    {
        public string Name => "customerJobListener";
        private readonly Dictionary<string, Stopwatch> _stopWatches = new Dictionary<string, Stopwatch>();
        private readonly JobMangerDal _jobMangerDal;

        public JobListener()
        {
            _jobMangerDal = new JobMangerDal();
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
            var jobId = int.Parse(context.JobDetail.JobDataMap["jobId"].ToString());
            try
            {
                var jobDetail = _jobMangerDal.GetJobDetailsById(jobId);

                if (jobDetail == null)
                {
                    throw new JobExecutionException("数据库中未找到该job");
                }

                if (string.IsNullOrWhiteSpace(jobDetail.InvocationData))
                {
                    throw new JobExecutionException("InvocationData为空");
                }

                context.JobDetail.JobDataMap.Put("invocationData", jobDetail.InvocationData);
                context.JobDetail.JobDataMap.Put("jobName", jobDetail.JobName);
                context.JobDetail.JobDataMap.Put("exceptionEmail", jobDetail.ExceptionEmail);
                context.JobDetail.JobDataMap.Put("jobGroupName", jobDetail.JobGroupName);

                var stopwatch = new Stopwatch();
                stopwatch.Start();
                _stopWatches.Add(context.FireInstanceId, stopwatch);

                jobExecuteLogs.JobDetailId = jobDetail.Id;
                jobExecuteLogs.JobName = jobDetail.JobName;
                jobExecuteLogs.JobGroupName = jobDetail.JobGroupName;
                jobExecuteLogs.Message = $"JobName:{jobDetail.JobName},发送调度";
                jobExecuteLogs.IsSuccess = true;

                exceptionEmail = jobDetail.ExceptionEmail;
            }
            catch (Exception ex)
            {
                jobExecuteLogs.JobDetailId = jobId;
                jobExecuteLogs.Message = $"发送调度出现异常:{ex.Message}";
                jobExecuteLogs.IsSuccess = false;
                jobExecuteLogs.ExceptionMessage = ex.Message;
                jobExecuteLogs.ExceptionStack = ex.StackTrace;
            }

            jobExecuteLogs.ExecuteInstanceIp = GetIp();
            jobExecuteLogs.ExecuteInstanceName = context.Scheduler.SchedulerInstanceId;
            jobExecuteLogs.CreationTime = DateTime.Now;

            _jobMangerDal.InsertJobExecuteLogs(jobExecuteLogs);
            if (!jobExecuteLogs.IsSuccess && !string.IsNullOrWhiteSpace(exceptionEmail))
            {
                SendExceptionEmail(exceptionEmail, jobExecuteLogs.JobName, jobExecuteLogs.ExceptionMessage, jobExecuteLogs.ExceptionStack);
            }

            await Task.CompletedTask;
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

                var elapsed = _stopWatches[context.FireInstanceId].ElapsedMilliseconds;


                jobExecuteLogs.Message = $"JobName:{context.JobDetail.JobDataMap["jobName"]},调度结束。调度响应时间:{elapsed}ms";
                jobExecuteLogs.IsSuccess = true;
            }
            catch (Exception ex)
            {
                jobExecuteLogs.Message = $"JobName:{context.JobDetail.JobDataMap["jobName"]},调度出现异常:{ex.Message}";
                jobExecuteLogs.IsSuccess = false;
                jobExecuteLogs.ExceptionMessage = GetAllExceptionInfo(ex);
                jobExecuteLogs.ExceptionStack = ex.StackTrace;
            }

            jobExecuteLogs.ExecuteInstanceIp = GetIp();
            jobExecuteLogs.ExecuteInstanceName = context.Scheduler.SchedulerInstanceId;
            jobExecuteLogs.CreationTime = DateTime.Now;

            _jobMangerDal.InsertJobExecuteLogs(jobExecuteLogs);
            if (!jobExecuteLogs.IsSuccess && !string.IsNullOrWhiteSpace(exceptionEmail))
            {
                SendExceptionEmail(exceptionEmail, jobExecuteLogs.JobName, jobExecuteLogs.ExceptionMessage, jobExecuteLogs.ExceptionStack);
            }

            await Task.CompletedTask;
        }

        private void SendExceptionEmail(string email, string jobName, string exceptionMessage, string exceptionStack)
        {
            
        }

        private string GetIp()
        {
            var alAllLocalIp = string.Empty;
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName); //取得本机IP
            for (int i = 0; i < ipEntry.AddressList.Length; i++)
            {
                if (ipEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    alAllLocalIp += ipEntry.AddressList[i] + ";";
            }

            return alAllLocalIp;
        }


        private static string GetAllExceptionInfo(Exception e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(e.Message);
            sb.AppendLine(e.ToString());

            var ex = e.InnerException;

            while (ex != null)
            {
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.ToString());
                ex = ex.InnerException;
            }
            return sb.ToString();
        }
    }
}