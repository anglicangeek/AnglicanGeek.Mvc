using System;
using System.Web.Mvc;
using AnglicanGeek.SimpleContainer;
using MvcDependencyResolver = System.Web.Mvc.DependencyResolver;

namespace AnglicanGeek.Mvc
{
    public class InjectingActionInvoker : ControllerActionInvoker
    {
        protected override object GetParameterValue(
            ControllerContext controllerContext,
            ParameterDescriptor parameterDescriptor)
        {
            if (!parameterDescriptor.ParameterType.IsAbstract && !parameterDescriptor.ParameterType.IsInterface)
                return base.GetParameterValue(controllerContext, parameterDescriptor);
            
            try
            {
                return GetParameterValueFromDependencyResolver(
                    parameterDescriptor.ParameterType, 
                    parameterDescriptor.ParameterName);
            }
            catch 
            {
                return null;
            }
        }

        private static object GetParameterValueFromDependencyResolver(
            Type type,
            string name)
        {
            var namedDependencyResolver = MvcDependencyResolver.Current.GetService<INamedDependencyResolver>();

            if (namedDependencyResolver != null)
                return namedDependencyResolver.GetService(type, name);
            else
                return MvcDependencyResolver.Current.GetService(type);
        }
    }
}
