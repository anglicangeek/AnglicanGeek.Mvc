using System;
using System.Web.Mvc;

namespace AnglicanGeek.Mvc
{
    public interface IScopedFilter
    {
        int? Order { get; }
        FilterScope Scope { get; }

        bool AppliesToContext(
            ControllerContext controllerContext, 
            ActionDescriptor actionDescriptor);
    }
}