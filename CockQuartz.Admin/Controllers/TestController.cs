using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CockQuartz.Application;

namespace CockQuartzAdmin.Controllers
{
    public class TestController : ApiController
    {
        [Route("CheckUserUserSummary")]
        [ApiJob(
            ApiJobDeveloper = "xijianyao",
            ApiJobDeveloperMail = "cong.sun@1hai.cn",
            ApiJobExceptionOnlyToMe = false,            
            ApiJobDescription = "用户UserSummary检查")]
        public string Get()
        {
            return "value";
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