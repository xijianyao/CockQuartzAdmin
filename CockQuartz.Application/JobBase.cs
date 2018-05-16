using System.Threading.Tasks;
using Quartz;

namespace CockQuartz.Core
{
    public  class JobBase : IJob
    {

        public JobBase()
        {
        }

        public async Task Execute(IJobExecutionContext context)
        {

        }
    }
}
