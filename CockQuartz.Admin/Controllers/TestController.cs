using System;
using System.Web.Http;
using CockQuartz.Application;
using FeI.Dependency;
using ServiceClients;

namespace CockQuartz.Admin.Controllers
{
    public class TestController2 : ApiController
    {
        private readonly IServiceClient _serviceClient;

        public TestController2(IServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
        }

        [Route("CheckUserUserSummary2")]
        [ApiJob(
            name: "第二个Job",
            developer: "xijianyao",
            developerMail: "cong.sun@1hai.cn",
            description: "用户UserSummary检查")]
        public void Get()
        {
            _serviceClient.Request<string>("http://baidu.com", HttpVerb.Get, new { });
            Console.WriteLine("111");
        }
    }

    public class TestController : ApiController
    {
        [Route("CheckUserUserSummary")]
        [ApiJob(
            name: "第一个Job",
            developer: "xijianyao",
            developerMail: "cong.sun@1hai.cn",
            description: "用户UserSummary检查")]
        public void Get()
        {
            var a = IocManager.Instance.Resolve(typeof(TestController2));

            var b = IocManager.Instance.Resolve<TestController2>();

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