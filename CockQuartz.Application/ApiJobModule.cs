using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CockQuartz.Core.Infrastructure;
using NLog;

namespace CockQuartz.Core
{
    /// <summary>
    /// ApiJobHelpHandler
    /// </summary>
    internal sealed class ApiJobModule : IHttpModule
    {


        #region 方法

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                //throw new NotImplementedException();
            }
            catch
            {

            }

        }


        /// <summary>
        /// 初始事件
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {

            context.BeginRequest += new EventHandler(context_BeginRequest);
            context.EndRequest += new EventHandler(context_EndRequest);

        }


        public bool IsTrue = false;//是否阻止webapi请求

        /// <summary>
        /// 请求事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void context_BeginRequest(object sender, EventArgs e)
        {
            if (ApiJobSettings.ApiJobSchedulerEnabled)
            {
                HttpApplication application = (HttpApplication)sender;


                var webUrl = application.Context.Request.Url.ToString();

                if (!webUrl.ToLower().Contains("jobmanagement") || !webUrl.ToLower().Contains("home/index"))
                {
                    IsTrue = true;
                    application.CompleteRequest();
                    application.Context.Response.Write("请求被终止。");
                }
            }

        }

        /// <summary>
        /// End事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void context_EndRequest(object sender, EventArgs e)
        {

        }



        #endregion
    }
}
