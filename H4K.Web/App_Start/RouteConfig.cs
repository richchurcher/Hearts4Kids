using System.Web.Mvc;
using System.Web.Routing;

namespace H4K.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Improve SEO by stopping duplicate URL's due to case differences or trailing slashes.
            // See http://googlewebmastercentral.blogspot.co.uk/2010/04/to-slash-or-not-to-slash.html
            routes.AppendTrailingSlash = true;
            routes.LowercaseUrls = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("Scripts/{*pathInfo}");
            routes.IgnoreRoute("Error/Forbidden.html");
            routes.IgnoreRoute("Error/GatewayTimeout.html");
            routes.IgnoreRoute("Error/ServiceUnavailable.html");

            routes.MapMvcAttributeRoutes();

            // Normal routes are not required because we are using attribute routing. So we don't need this MapRoute 
            // statement. Unfortunately, Elmah.MVC has a bug in which some 404 and 500 errors are not logged without 
            // this route in place. So we include this but look out on these pages for a fix:
            // https://github.com/alexbeletsky/elmah-mvc/issues/60
            // https://github.com/ASP-NET-MVC-Boilerplate/Templates/issues/8
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
