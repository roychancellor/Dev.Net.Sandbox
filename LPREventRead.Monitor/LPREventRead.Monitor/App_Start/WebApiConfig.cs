using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LPREventRead.Monitor
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "MonitorAPI",
                routeTemplate: "api/v1/LPRSubmitter/Monitor/IdleMinutes",
                defaults: new { id = RouteParameter.Optional, controller = "Monitor", action = "IdleMinutes" }
            );
        }
    }
}
