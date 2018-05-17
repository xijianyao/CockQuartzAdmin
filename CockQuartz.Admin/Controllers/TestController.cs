using System;
using System.Linq;
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

        [Route("CheckUserUserSummary")]
        [ApiJob(name: "CheckUserUserSummary", apiJobDeveloper: "xijianyao ")]
        public void Get()
        {
            var a = _jobMangerDal.GetJobDetails();
            throw new Exception(a.FirstOrDefault()?.Cron);
        }

        [Route("SendRequest")]
        [ApiJob(name: "SendRequest", apiJobDeveloper: "xijianyao1")]
        public void SendRequest()
        {
            _serviceClient.Request<string>("http://www.baidu.com", HttpVerb.Get, new { });
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