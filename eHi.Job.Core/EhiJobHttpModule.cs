using System;
using System.Text;
using System.Web;
using eHi.Job.Core.Infrastructure;

namespace eHi.Job.Core
{
    /// <summary>
    /// ApiJobHelpHandler
    /// </summary>
    internal sealed class EhiJobHttpModule : IHttpModule
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

                if (!webUrl.ToLower().Contains("jobmanagement") 
                    && !webUrl.ToLower().Contains("home/index")
                    && !webUrl.ToLower().Contains(".css")
                    && !webUrl.ToLower().Contains(".css")
                    && !webUrl.ToLower().Contains(".js")
                    && !webUrl.ToLower().Contains("swagger"))
                {
                    application.CompleteRequest();
                    application.Context.Response.Charset = "utf-8";
                    application.Context.Response.ContentEncoding = Encoding.UTF8;
                    application.Context.Response.ContentType = "application/xml";
                    application.Context.Response.Write("Job启动，业务webapi请求被终止。");
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
