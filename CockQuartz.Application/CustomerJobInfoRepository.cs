using System;
using System.Linq;
using System.Linq.Expressions;
using CockQuartz.Interface;
using CockQuartz.Model;

namespace CockQuartz.Application
{
    public class CustomerJobInfoRepository : ICustomerJobInfoRepository
    {
        public int AddCustomerJobInfo(string jobName, string jobGroupName, string triggerName, string triggerGroupName, string cron,
            string jobDescription, string requestUrl)
        {
            throw new NotImplementedException();
        }

        public int UpdateCustomerJobInfo(JobDetail Customer_JobInfoModel)
        {
            throw new NotImplementedException();
        }

        public Tuple<IQueryable<JobDetail>, int> LoadCustomerInfoes<K>(Expression<Func<JobDetail, bool>> whereLambda, Expression<Func<JobDetail, K>> orderByLambda, bool isAsc, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public JobDetail LoadCustomerInfo(Expression<Func<JobDetail, bool>> whereLambda)
        {
            throw new NotImplementedException();
        }
    }
}
