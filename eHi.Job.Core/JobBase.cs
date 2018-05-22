using System;
using System.Reflection;
using System.Threading.Tasks;
using Quartz;

namespace eHi.Job.Core
{
    public class JobBase : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var jobId = int.Parse(context.JobDetail.JobDataMap["jobId"].ToString());
            var method = EhiJobCoreModule.MethodsDic[jobId];

            object instance = Activator.CreateInstance(method.DeclaringType ?? throw new InvalidOperationException());
            method.Invoke(instance, BindingFlags.OptionalParamBinding | BindingFlags.InvokeMethod, null, null, System.Globalization.CultureInfo.CurrentCulture);

            await Task.CompletedTask;
        }

    }
}
