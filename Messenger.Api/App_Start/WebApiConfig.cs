using System.Web.Http;

namespace Messenger.Api
{
#pragma warning disable 1591
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
#pragma warning restore 1591
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );
        }
    }
}