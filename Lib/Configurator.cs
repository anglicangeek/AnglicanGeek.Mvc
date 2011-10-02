using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcDependencyResolver = System.Web.Mvc.DependencyResolver;
using MvcRazorViewEngine = System.Web.Mvc.RazorViewEngine;
using MvcWebFormViewEngine = System.Web.Mvc.WebFormViewEngine;

namespace AnglicanGeek.Mvc
{
    public static class Configurator
    {
        public static void FixUpViewEngines()
        {
            var viewEngines = ViewEngines.Engines;
            if (viewEngines.Count != 2)
                return;

            var razorViewEngine = viewEngines.Where(x => x.GetType() == typeof(MvcRazorViewEngine)).SingleOrDefault();
            var webFormViewEngine = viewEngines.Where(x => x.GetType() == typeof(MvcWebFormViewEngine)).SingleOrDefault();

            if (razorViewEngine != null && webFormViewEngine != null)
            {
                viewEngines.Clear();
                viewEngines.Add(new WebFormViewEngine());
                viewEngines.Add(new RazorViewEngine());
            }
        }

        public static void UseActionInjection()
        {
            if (MvcDependencyResolver.Current == null)
                throw new InvalidOperationException("Cannot use action injection without a registered dependency resolver.");

            var dependencyResolver = MvcDependencyResolver.Current as IDependencyRegistry;
            if (dependencyResolver == null)
                throw new InvalidOperationException("Cannot automatically register action injection for the current dependency resolver. You can manually register the ActionInjectionControllerActivator for your dependency resolver to use action injection.");
            else
                dependencyResolver.RegisterCreator<IControllerActivator>(() => new ActionInjectionControllerActivator());
        }

        public static void UseSimpleDependencyContainer()
        {
            var dependencyResolver = new SimpleDependencyContainer();

            var registrarInterface = typeof(IDependencyRegistrar);

            var registrars = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type != registrarInterface && registrarInterface.IsAssignableFrom(type))
                .Select(type => Activator.CreateInstance(type) as IDependencyRegistrar);

            if (registrars.Count() > 0)
            {
                foreach (var registrar in registrars)
                    registrar.RegisterDependencies(dependencyResolver);

                MvcDependencyResolver.SetResolver(dependencyResolver);
            }
        }

        public static void UseRouteRegistrars()
        {
            var registrarInterface = typeof(IRouteRegistrar);

            var registrars = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type != registrarInterface && registrarInterface.IsAssignableFrom(type))
                .Select(type => Activator.CreateInstance(type) as IRouteRegistrar);

            foreach (var registrar in registrars)
                registrar.RegisterRoutes(RouteTable.Routes);
        }

        public static void UseScopedFilters()
        {
            if (MvcDependencyResolver.Current != null)
            {
                var dependencyResolver = MvcDependencyResolver.Current;
                
                if (dependencyResolver is IDependencyRegistry)
                    ((IDependencyRegistry)dependencyResolver).RegisterCreator<IFilterProvider>(() => new FilterProvider());
            }
        }
    }
}