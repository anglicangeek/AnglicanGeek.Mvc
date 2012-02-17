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
            object parameter = null;

            try
            {
                parameter = GetParameterValueFromDependencyResolver(
                    parameterDescriptor.ParameterType,
                    parameterDescriptor.ParameterName);
            }
            catch
            {
                return null;
            }

            if (parameter == null)
                parameter = base.GetParameterValue(controllerContext, parameterDescriptor);

            return parameter;
        }

        private static object GetParameterValueFromDependencyResolver(
            Type type,
            string name)
        {
            var dependencyRegistry = MvcDependencyResolver.Current.GetService<IDependencyRegistry>();
            var namedDependencyResolver = MvcDependencyResolver.Current.GetService<INamedDependencyResolver>();

            if (dependencyRegistry == null)
            {
                if (type.IsAbstract || type.IsInterface)
                    return MvcDependencyResolver.Current.GetService(type);
                else
                    return null;
            }

            if (!dependencyRegistry.TypeIsRegistered(type))
                return null;

            if (namedDependencyResolver != null)
                return namedDependencyResolver.GetService(type, name);
            else
                return MvcDependencyResolver.Current.GetService(type);
        }
    }
}
