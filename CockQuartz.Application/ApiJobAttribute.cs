using System;

namespace CockQuartz.Application
{
    /// <summary>
    /// ApiJob 的描述信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class ApiJobAttribute : Attribute
    {
        /// <summary>
        /// 私有构造函数
        /// </summary>
        public ApiJobAttribute() { }

        /// <summary>
        /// Apijob control
        /// </summary>
        /// <param name="developer">开发者名称</param>
        /// <param name="developerMail">开发者邮件</param>
        /// <param name="exceptionOnlyToMe">该Job报错是否只发送给我，True只发送给我，False web.config中配置的ApiJobExceptionMailTo中的邮箱账号都得发送。</param>
        /// <param name="apiJobDescription">ApiJob的描述，4000字内</param>
        /// <param name="quartzCronExpression">该Job的调度计划，使用Quartz 的Cron Expression 来调度作业</param>
        public ApiJobAttribute(string developer, string developerMail, bool exceptionOnlyToMe, string apiJobDescription, string quartzCronExpression)
        {
            this.ApiJobDeveloper = developer;
            this.ApiJobDeveloperMail = developerMail;
            this.ApiJobExceptionOnlyToMe = exceptionOnlyToMe;
            this.ApiJobDescription = apiJobDescription;
            this.ApiJobQuartzCronExpression = quartzCronExpression;
        }


        /// <summary>
        /// 开发者
        /// </summary>
        public string ApiJobDeveloper { get; set; }


        /// <summary>
        /// 开发者邮箱,用于报错通知
        /// </summary>
        public string ApiJobDeveloperMail { get; set; }


        /// <summary>
        /// 该Job的异常是不是只发送给我
        /// true:   邮件只发送给ApiJobDeveloperMail 以及抄送给 web.config 中的   ApiJobExceptionMailCC。
        /// false:  邮件同时发送给我以及web.config 中的   ApiJobExceptionMailTo 以及抄送给 web.config 中的   ApiJobExceptionMailCC
        /// </summary>
        public bool ApiJobExceptionOnlyToMe { get; set; }

        /// <summary>
        /// Job描述，不得少于50字
        /// </summary>
        public string ApiJobDescription { get; set; }

        /// <summary>
        /// 调度计划
        /// 具体调度计划如何设置请参照:  http://www.quartz-scheduler.net/documentation/quartz-2.x/tutorial/crontrigger.html
        /// </summary>
        public string ApiJobQuartzCronExpression { get; set; }
    }
}
