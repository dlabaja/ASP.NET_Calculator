﻿using System.Web.Mvc;
using System.Web.Routing;

namespace ASP.NET_Calculator
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                namespaces: new[] { "ASP.NET_Calculator.Controllers" }
            );
        }
    }
}