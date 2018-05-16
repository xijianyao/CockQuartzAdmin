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
        public ApiJobAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Job名字
        /// </summary>
        public string Name { get; set; }

    }
}
