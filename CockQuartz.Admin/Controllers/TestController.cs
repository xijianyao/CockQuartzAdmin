using System;
using System.Web.Http;
using CockQuartz.Core.Infrastructure;

namespace CockQuartz.Admin.Controllers
{
    public class TestController : ApiController
    {

        [Route("CheckUserUserSummary")]
        [ApiJob("CheckUserUserSummary")]
        public void Get()
        {
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