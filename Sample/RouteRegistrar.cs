using System.Web.Mvc;
using System.Web.Routing;
using AnglicanGeek.Mvc;

namespace Sample
{
    public class RouteRegistrar : IRouteRegistrar
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", 
                "{controller}/{action}/{id}", 
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}