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
using eHi.Library.Dto;
using eHi.Library.Interface;
using eHi.Library.Service;
using Quartz;

namespace CockQuartz.Core.Infrastructure
{
    public class JobListener : IJobListener
    {
        public string Name => "customerJobListener";
        private readonly Dictionary<string, Stopwatch> _stopWatches = new Dictionary<string, Stopwatch>();
        private readonly JobMangerDal _jobMangerDal;
        private readonly IMessageService _messageService;

        public JobListener()
        {
            _jobMangerDal = new JobMangerDal();
            _messageService = new MessageService();
        }

        public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Task.CompletedTask;
        }

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _stopWatches.Add(context.FireInstanceId, stopwatch);

            await Task.CompletedTask;
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            var jobId = int.Parse(context.JobDetail.JobDataMap["jobId"].ToString());
            var jobExecuteLogs =
                new JobExecuteLogs
                {
                    JobDetailId = jobId
                };

            var exceptionEmail = string.Empty;
            try
            {
                var jobDetail = _jobMangerDal.GetJobDetailsById(jobId);
                if (jobDetail == null)
                {
                    throw new Exception($"未找到此Job，JobId{jobId}");
                }

                jobExecuteLogs.JobName = jobDetail.JobName;
                jobExecuteLogs.JobGroupName = jobDetail.JobGroupName;
                exceptionEmail = jobDetail.ExceptionEmail;
                jobExecuteLogs.Message = $"JobName:{context.JobDetail.JobDataMap["jobName"]},执行结束。";
                jobExecuteLogs.IsSuccess = true;


                if (jobException != null)
                {
                    throw jobException;
                }

            }
            catch (Exception ex)
            {
                jobExecuteLogs.Message = $"JobName:{context.JobDetail.JobDataMap["jobName"]},执行出现异常:{ex.Message}。";
                jobExecuteLogs.IsSuccess = false;
                jobExecuteLogs.ExceptionMessage = GetAllExceptionInfo(ex);
            }
            finally
            {
                var elapsed = _stopWatches[context.FireInstanceId].ElapsedMilliseconds;
                jobExecuteLogs.ExecuteInstanceIp = GetIp();
                jobExecuteLogs.ExecuteInstanceName = context.Scheduler.SchedulerInstanceId;
                jobExecuteLogs.CreationTime = DateTime.Now;
                jobExecuteLogs.Duration = elapsed;
            }

            _jobMangerDal.InsertJobExecuteLogs(jobExecuteLogs);
            if (!jobExecuteLogs.IsSuccess)
            {
                SendExceptionEmail(string.IsNullOrWhiteSpace(exceptionEmail) ? ApiJobSettings.ApiJobExceptionMailTo : exceptionEmail,
                    jobExecuteLogs.Message + jobExecuteLogs.ExceptionMessage);
            }

            await Task.CompletedTask;
        }

        private void SendExceptionEmail(string email, string exceptionMessage)
        {
            _messageService.SendEmail(new EmailDto
            {
                SendTo = email,
                EmailId = 300014,
                OperatorId = "ApiJob",
                Parameters = new Dictionary<string, string>
                    {
                        {"Content",exceptionMessage}
                    }
            });
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