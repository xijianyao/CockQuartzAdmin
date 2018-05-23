using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eHi.Common.Exception;
using eHi.Job.Core.Dto;
using eHi.Job.Core.JobManager;
using eHi.Job.Core.Models;
using eHi.Library.Dto;
using eHi.Library.Interface;
using eHi.Library.Service;
using Quartz;

namespace eHi.Job.Core.Infrastructure
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

            var jobId = int.Parse(context.JobDetail.JobDataMap["jobId"].ToString());
            var jobDetail = _jobMangerDal.GetJobDetailsById(jobId);

            if (jobDetail.ExecuteStatus == ExecuteStatusType.Executing)
            {
                //添加日志
                var jobExecuteLogsExecuting =
                    new JobExecuteLogs
                    {
                        JobDetailId = jobId,
                        JobName = jobDetail.JobName,
                        JobGroupName = jobDetail.JobGroupName,
                        Message = $"Job：{jobDetail.JobGroupName}_{jobDetail.JobName}正在执行中，启动失败。请开发者确认Job执行状态是否正常，或者执行计划是否大于执行时间。",
                        ExecuteInstanceIp = GetIp(),
                        ExecuteInstanceName = context.Scheduler.SchedulerInstanceId,
                        CreationTime = DateTime.Now,
                        IsSuccess = false
                    };
                var jobExecutingLogId = _jobMangerDal.InsertJobExecuteLogs(jobExecuteLogsExecuting);
                context.JobDetail.JobDataMap.Put("jobLogId", jobExecutingLogId);
                Task.Run(() =>
                {
                    SendExceptionEmail(string.IsNullOrWhiteSpace(jobDetail.ExceptionEmail) ? ApiJobSettings.ApiJobExceptionMailTo : jobDetail.ExceptionEmail,
                        $"Job：{jobDetail.JobGroupName}_{jobDetail.JobName}正在执行中，启动失败。请开发者确认Job状态是否正常，或者执行计划是否大于执行时间。");
                });
                throw new StringResponseException($"Job：{jobDetail.JobGroupName}_{jobDetail.JobName}正在运行中，启动失败");
            }

            //更新Job执行状态
            jobDetail.ExecuteStatus = ExecuteStatusType.Executing;
            _jobMangerDal.UpdateJobDetail(jobDetail);

            //添加日志
            var jobExecuteLogs =
                new JobExecuteLogs
                {
                    JobDetailId = jobId,
                    JobName = jobDetail.JobName,
                    JobGroupName = jobDetail.JobGroupName,
                    Message = $"Job:{jobDetail.JobGroupName}_{jobDetail.JobName},正在执行中...",
                    ExecuteInstanceIp = GetIp(),
                    ExecuteInstanceName = context.Scheduler.SchedulerInstanceId,
                    CreationTime = DateTime.Now,
                    IsSuccess = false
                };
            var jobLogId = _jobMangerDal.InsertJobExecuteLogs(jobExecuteLogs);
            context.JobDetail.JobDataMap.Put("jobLogId", jobLogId);

            await Task.CompletedTask;
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            var jobId = int.Parse(context.JobDetail.JobDataMap["jobId"].ToString());
            var jobLogId = int.Parse(context.JobDetail.JobDataMap["jobLogId"].ToString());
            var jobExecuteLogs =
                new JobExecuteLogs
                {
                    Id = jobLogId,
                    JobDetailId = jobId
                };
            var jobName = string.Empty;
            var jobGroupName = string.Empty;

            var exceptionEmail = string.Empty;
            try
            {
                var jobDetail = _jobMangerDal.GetJobDetailsById(jobId);
                if (jobDetail == null)
                {
                    throw new Exception($"未找到此Job，JobId{jobId}");
                }
                jobName = jobDetail.JobName;
                jobGroupName = jobDetail.JobGroupName;

                jobExecuteLogs.JobName = jobName;
                jobExecuteLogs.JobGroupName = jobDetail.JobGroupName;
                exceptionEmail = jobDetail.ExceptionEmail;
                jobExecuteLogs.Message = $"Job:{jobName},成功执行结束";
                jobExecuteLogs.IsSuccess = true;

                jobDetail.ExecuteStatus = ExecuteStatusType.WaitExecute;            
                _jobMangerDal.UpdateJobDetail(jobDetail);

                if (jobException != null)
                {
                    throw jobException;
                }

            }
            catch (Exception ex)
            {
                jobExecuteLogs.Message = $"Job:{jobGroupName}_{jobName},执行出现异常:{ex.Message}";
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

            _jobMangerDal.UpdateJobExecuteLogs(jobExecuteLogs);
            if (!jobExecuteLogs.IsSuccess)
            {
#pragma warning disable CS4014 // 由于此调用不会等待，因此在此调用完成之前将会继续执行当前方法。请考虑将 "await" 运算符应用于调用结果。
                Task.Run(() =>
                {
                    SendExceptionEmail(string.IsNullOrWhiteSpace(exceptionEmail) ? ApiJobSettings.ApiJobExceptionMailTo : exceptionEmail,
                        jobExecuteLogs.Message + jobExecuteLogs.ExceptionMessage);
                });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在此调用完成之前将会继续执行当前方法。请考虑将 "await" 运算符应用于调用结果。
            }

            await Task.CompletedTask;
        }

        private void SendExceptionEmail(string email, string exceptionMessage)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            var emailList = email.Split(';').ToList();
            foreach (var item in emailList)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    break;
                }
                _messageService.SendEmail(new EmailDto
                {
                    SendTo = item,
                    EmailId = 300014,
                    OperatorId = "ApiJob",
                    Parameters = new Dictionary<string, string>
                    {
                        {"Content",exceptionMessage}
                    }
                });
            }
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