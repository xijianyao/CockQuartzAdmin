using System;
using System.Linq;
using System.Linq.Expressions;
using CockQuartz.Interface;
using CockQuartz.Model;
using FeI.Application;
using FeI.Domain.Repositories;

namespace CockQuartz.Application
{
    public class CustomerJobInfoService : ApplicationService, ICustomerJobInfoService
    {
        private readonly IRepository<JobDetail> _jobDetailRepository;
        public CustomerJobInfoService(IRepository<JobDetail> jobDetailRepository)
        {
            _jobDetailRepository = jobDetailRepository;
        }


        public int AddCustomerJobInfo(string jobName, string jobGroupName, string triggerName, string triggerGroupName, string cron,
            string jobDescription, string requestUrl, string exceptionEmail)
        {
            var job = new JobDetail
            {
                JobName = "JobName",
                RequestUrl = "RequestUrl",
                CreateTime = DateTime.Now,
                TriggerName = "TriggerName",
                UpdateTime = DateTime.Now,
                UpdateUser = "UpdateUser",
                GroupName = "GroupName",
                ExceptionEmail = "ExceptionEmail",
                CreateUser = "CreateUser",
                Cron = "Cron",
                Description = "Description"
            };
            return _jobDetailRepository.InsertAndGetId(job);
        }

        public int UpdateCustomerJobInfo(JobDetail customerJobInfoModel)
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
