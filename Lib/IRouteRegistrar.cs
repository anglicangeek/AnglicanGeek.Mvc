using System;
using System.Web.Routing;

namespace AnglicanGeek.Mvc
{
    public interface IRouteRegistrar
    {
        void RegisterRoutes(RouteCollection routes);
    }
}
