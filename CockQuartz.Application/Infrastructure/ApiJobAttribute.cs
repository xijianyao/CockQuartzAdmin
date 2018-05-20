using System;

namespace CockQuartz.Core.Infrastructure
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
        /// <param name="apiJobName">Job名字</param>
        /// <param name="apiJobDeveloper">开发者</param>
        /// <param name="apiJobDeveloperMail">Job错误发送Email地址</param>
        /// <param name="apiJobDescription">Job描述</param>
        public ApiJobAttribute(string apiJobName, string apiJobDeveloper, string apiJobDeveloperMail, string apiJobDescription)
        {
            ApiJobName = apiJobName;
            ApiJobDeveloper = apiJobDeveloper;
            ApiJobDeveloperMail = apiJobDeveloperMail;
            ApiJobDescription = apiJobDescription;
        }

        /// <summary>
        /// Job名字
        /// </summary>
        public string ApiJobName { get; set; }

        /// <summary>
        /// 开发者
        /// </summary>
        public string ApiJobDeveloper { get; set; }

        /// <summary>
        /// Job错误发送Email地址
        /// </summary>
        public string ApiJobDeveloperMail { get; set; }

        /// <summary>
        /// Job描述
        /// </summary>
        public string ApiJobDescription { get; set; }
    }
}
