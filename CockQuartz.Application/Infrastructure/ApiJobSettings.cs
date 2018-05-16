using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CockQuartz.Core.Infrastructure
{
    public class ApiJobSettings
    {
        /// <summary>
        /// 需要扫描的程序集    test.dll
        /// </summary>
        public static string ApiJobAssemblyName => System.Configuration.ConfigurationManager.AppSettings["ApiJobAssemblyName"];

        /// <summary>
        /// 实例名字
        /// </summary>
        public static string QuartzInstanceName => System.Configuration.ConfigurationManager.AppSettings["QuartzInstanceName"];

        /// <summary>
        /// 系统名称
        /// </summary>
        public static string ApiJobSystemName => System.Configuration.ConfigurationManager.AppSettings["ApiJobSystemName"];

        /// <summary>
        /// Job运行错误,邮件通知
        /// </summary>
        internal static string ApiJobExceptionMailTo
        {
            get
            {
                string value = System.Configuration.ConfigurationManager.AppSettings["ApiJobExceptionMailTo"];
                if (string.IsNullOrEmpty(value))
                {
                    return "";
                }
                return value.Trim();
            }
        }
    }
}
