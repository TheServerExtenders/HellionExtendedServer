using System.Net.Http;
using System.Web;

namespace HellionExtendedServer.WebAPI
{
    public static class UtilityMethods
    {
        public static string GetUserIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var ctx = request.Properties["MS_HttpContext"] as HttpContextBase;
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            return null;
        }

    }
}
