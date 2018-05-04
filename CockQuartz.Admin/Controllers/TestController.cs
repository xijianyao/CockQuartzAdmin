using System;
using System.Web.Http;
using CockQuartz.Application;

namespace CockQuartz.Admin.Controllers
{
    public class TestController : ApiController
    {
        [Route("CheckUserUserSummary")]
        [ApiJob(
            name:"第一个Job",
            developer:"xijianyao",
            developerMail : "cong.sun@1hai.cn",
            description: "用户UserSummary检查")]
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