using eHi.Library.Common;
using eHi.Library.Enum;

namespace CockQuartz.Admin.Common
{
    public class CockQuartzConst
    {
        static CockQuartzConst()
        {
            if (EnvironmentConfiguration.Environment == EnvironmentType.Product)
            {

                StaticUrl = "http://resource.1hai.cn/resource/";
            }
            else
            {

                StaticUrl = "http://demo5.1hai.cn/resource/";
            }
        }

        /// <summary>
        ///     静态资源URL
        /// </summary>
        public static string StaticUrl { get; set; }
    }
}