using System.Web.Mvc;
using System.Web.Routing;

namespace POMS
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");

            routes.MapRoute(
             name: "InvoiceEntry",
             url: "InvoiceEntry_Tax",
             defaults: new { controller = "Invoice", action = "InvoiceEntry_Tax", id = UrlParameter.Optional }
         );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default1",
                url: "{a}/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional,a =UrlParameter.Optional }
            );

          




        }
    }
}