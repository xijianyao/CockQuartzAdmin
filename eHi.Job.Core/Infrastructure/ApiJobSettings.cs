﻿namespace eHi.Job.Core.Infrastructure
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
        /// 是否启动Job，0不启动，1启动Job阻止webapi请求（除Jobmanagement和home控制器外），
        /// </summary>
        public static bool ApiJobSchedulerEnabled
        {
            get
            {
                string value = System.Configuration.ConfigurationManager.AppSettings["ApiJobSchedulerEnabled"];
                if (!string.IsNullOrEmpty(value) && (value.Trim() == "1" || value.ToLower().Trim() == "true"))
                {
                    return true;
                }
                return false;
            }
        }


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