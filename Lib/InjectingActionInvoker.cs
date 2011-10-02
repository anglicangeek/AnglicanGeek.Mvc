using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

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
                    parameterValue = DependencyResolver.Current.GetService(parameterDescriptor.ParameterType);
                }
                catch { }
            }

            return parameterValue;
        }
    }
}
