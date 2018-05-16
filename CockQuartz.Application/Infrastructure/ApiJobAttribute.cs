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
        /// Apijob control
        /// </summary>
        /// <param name="name">Job名字</param>
        /// <param name="apiJobDeveloper">开发者</param>
        public ApiJobAttribute(string name, string apiJobDeveloper)
        {
            Name = name;
            ApiJobDeveloper = apiJobDeveloper;
        }

        /// <summary>
        /// Job名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 开发者
        /// </summary>
        public string ApiJobDeveloper { get; set; }

    }
}
