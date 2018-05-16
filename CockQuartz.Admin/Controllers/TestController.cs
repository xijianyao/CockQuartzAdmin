using System;
using System.Web.Http;
using CockQuartz.Core.Infrastructure;
using CockQuartz.Core.JobManager;

namespace CockQuartz.Admin.Controllers
{
    public class TestController : ApiController
    {
        private readonly JobMangerDal _jobMangerDal;

        public TestController()
        {
            _jobMangerDal = new JobMangerDal();
        }

        [Route("CheckUserUserSummary")]
        [ApiJob(name: "CheckUserUserSummary", apiJobDeveloper: "xijianyao ")]
        public void Get()
        {
            var a = _jobMangerDal.GetJobDetails();
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