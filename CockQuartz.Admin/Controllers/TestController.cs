using System;
using System.Linq;
using System.Threading;
using System.Web.Http;
using System.Web.Services.Description;
using CockQuartz.Core.Infrastructure;
using CockQuartz.Core.JobManager;
using ServiceClients;

namespace CockQuartz.Admin.Controllers
{
    public class TestController : ApiController
    {
        private readonly JobMangerDal _jobMangerDal;
        private readonly IServiceClient _serviceClient;

        public TestController()
        {
            _jobMangerDal = new JobMangerDal();
            _serviceClient = new ServiceClient();
        }

        [Route("CheckUserSummaryError")]
        [ApiJob(
            ApiJobName = "检查用户Summary",
            ApiJobDeveloper = "xijianyao",
            ApiJobDeveloperMail = "jianyao.xi@1hai.cn",
            ApiJobDescription = "检查用户Summary描述"
            )]
        public void Get()
        {
            var a = _jobMangerDal.GetJobDetails();
            throw new Exception(a.FirstOrDefault()?.Cron);
        }

        [Route("SendRequest")]
        [ApiJob(
            ApiJobName = "请求百度接口",
            ApiJobDeveloper = "xijianyao",
            ApiJobDeveloperMail = "jianyao.xi@1hai.cn",
            ApiJobDescription = "请求百度接口描述"
        )]
        public void SendRequest()
        {
            new ServiceClient().Request("http://www.baidu.com", HttpVerb.Get, new { });
            Console.WriteLine("111");
        }

        //[Route("SendRequest1")]
        [ApiJob(
            ApiJobName = "睡眠十秒",
            ApiJobDeveloper = "xijianyao",
            ApiJobDeveloperMail = "jianyao.xi@1hai.cn",
            ApiJobDescription = "睡眠十秒"
        )]
        public void SendRequest1()
        {
            new ServiceClient().Request("http://www.baidu.com", HttpVerb.Get, new { });
            Thread.Sleep(10000);
            Console.WriteLine("111");
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

    }
}