using System.Web;
using System.Web.Http;

namespace Messenger.Api
{
#pragma warning disable 1591
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
#pragma warning restore 1591
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}