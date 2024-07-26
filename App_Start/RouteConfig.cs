using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CallBackUtility
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*(CrystalImageHandler).*" });
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Users", action = "Index", id = UrlParameter.Optional }
            //);
            routes.MapRoute(
      name: "Default",
      url: "{controller}/{action}/{id}",
      defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
  );
            routes.MapRoute(
              name: "Recordings",
              url: "{controller}/{action}/{id}",
              defaults: new { controller = "Recordings", action = "Index", id = UrlParameter.Optional }
          );
            routes.MapRoute(
           name: "Error",
           url: "Error/{action}/{id}",
           defaults: new { controller = "Error", action = "Index", id = UrlParameter.Optional }
       );

        }
    }
}
