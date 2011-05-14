using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AnglicanGeek.Mvc
{
    public class FilterProvider : IFilterProvider
    {
        readonly IEnumerable<IScopedFilter> scopedFilters;

        public FilterProvider()
        {
            var scopedFilterInterface = typeof(IScopedFilter);

            this.scopedFilters = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type != scopedFilterInterface && scopedFilterInterface.IsAssignableFrom(type))
                .Select(type => DependencyResolver.Current.GetService(type) as IScopedFilter)
                .Where(filter =>
                    filter is IActionFilter ||
                    filter is IResultFilter ||
                    filter is IExceptionFilter ||
                    filter is IAuthorizationFilter);
        }
        
        public IEnumerable<Filter> GetFilters(
            ControllerContext controllerContext, 
            ActionDescriptor actionDescriptor)
        {
            // TODO: Consider caching calls to AppliesToContext if there is a reliable set of cache keys
            return scopedFilters
                .Where(filter => filter.AppliesToContext(controllerContext, actionDescriptor))
                .Select(filter => new Filter(filter, filter.Scope, filter.Order));
        }
    }
}