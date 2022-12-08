using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MultiStateQueryPOC
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // For some reason, this is the only way this will accept and deserialize XML.
            // I have tried 8 ways to Sunday and changing the XML serializer is the only thing that works.
            var xml = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
            xml.UseXmlSerializer = true;

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
