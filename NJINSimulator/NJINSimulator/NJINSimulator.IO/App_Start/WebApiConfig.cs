using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace NJINSimulator.IO
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Formatters.XmlFormatter.UseXmlSerializer = true; // This is necessary to deserialize XML in **REQUESTS**

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "njinsimulator/io/api/v1/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
