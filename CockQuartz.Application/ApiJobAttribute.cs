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
        /// Apijob control
        /// </summary>
        /// <param name="name">Job名字</param>
        /// <param name="developer">开发者名称</param>
        /// <param name="developerMail">开发者邮件</param>
        /// <param name="description">ApiJob的描述，4000字内</param>
        public ApiJobAttribute(string name, string developer, string developerMail, string description)
        {
            Name = name;
            Developer = developer;
            DeveloperMail = developerMail;
            Description = description;
        }

        /// <summary>
        /// Job名字
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 开发者
        /// </summary>
        public string Developer { get; set; }

        /// <summary>
        /// 开发者邮箱,用于报错通知
        /// </summary>
        public string DeveloperMail { get; set; }

        /// <summary>
        /// Job描述，不得少于50字
        /// </summary>
        public string Description { get; set; }
    }
}
