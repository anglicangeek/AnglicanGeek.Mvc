using System;
using System.Web.Mvc;
using AnglicanGeek.Mvc;

namespace Sample
{
    public class MyGlobalActionFilter : IScopedFilter, IActionFilter
    {
        public int? Order
        {
            get { return null; }
        }

        public FilterScope Scope
        {
            get { return FilterScope.Global; }
        }

        public bool AppliesToContext(
            ControllerContext controllerContext, 
            ActionDescriptor actionDescriptor)
        {
            return true;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.ViewBag.Fnord = "fnord";
        }
    }
}