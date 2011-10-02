using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace AnglicanGeek.Mvc
{
    public class ActionInjectionControllerActivator : IControllerActivator
    {
        public IController Create(
            RequestContext requestContext, 
            Type controllerType)
        {
            var controller = DependencyResolver.Current.GetService(controllerType) as IController;
            var typedController = controller as Controller;
            if (typedController != null)
                typedController.ActionInvoker = new InjectingActionInvoker();

            return controller;
        }
    }
}
