using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CockQuartz.RemoteServer
{
    public class JobListener : IJobListener
    {

        public string Name => "customerJobListener";

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            int jobId = 0;
            string name = string.Empty;
            try
            {
                jobId = Convert.ToInt32(context.JobDetail.JobDataMap["jobId"]);
                var triggerState = await context.Scheduler.GetTriggerState(context.Trigger.Key, cancellationToken);
                name = triggerState.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("异常信息：{0}", ex.Message);

            }

            Console.WriteLine("任务编号{0}；执行时间：{1},状态：{2}", jobId, DateTime.Now, name);
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {
            int jobId = 0;
            string name = string.Empty;
            try
            {
                jobId = Convert.ToInt32(context.JobDetail.JobDataMap["jobId"]);
                var triggerState = await context.Scheduler.GetTriggerState(context.Trigger.Key, cancellationToken);
                name = triggerState.ToString();

                Console.WriteLine("jobId{0}执行失败：{1}", context.JobDetail.JobDataMap["jobId"], jobException.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("异常{0}", e.Message);
            }
        }
    }
}
