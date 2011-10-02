using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MvcDependencyResolver = System.Web.Mvc.DependencyResolver;

namespace AnglicanGeek.Mvc
{
    public class InjectingActionInvoker : ControllerActionInvoker
    {
        protected override object GetParameterValue(
            ControllerContext controllerContext,
            ParameterDescriptor parameterDescriptor)
        {
            object parameterValue = null;

            if (!parameterDescriptor.ParameterType.IsAbstract && !parameterDescriptor.ParameterType.IsInterface)
                parameterValue = base.GetParameterValue(controllerContext, parameterDescriptor);
            
            if (parameterValue == null)
            {
                try
                {
                    parameterValue = GetParameterValueFromDependencyResolver(
                        parameterDescriptor.ParameterType, 
                        parameterDescriptor.ParameterName);
                }
                catch { }
            }

            return parameterValue;
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
