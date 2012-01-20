using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using AnglicanGeek.SimpleContainer;
using MvcDependencyResolver = System.Web.Mvc.DependencyResolver;
using MvcRazorViewEngine = System.Web.Mvc.RazorViewEngine;
using MvcWebFormViewEngine = System.Web.Mvc.WebFormViewEngine;

namespace AnglicanGeek.Mvc
{
    public static class Configurator
    {
        public static void FixUpViewEngines()
        {
            const string errorMessage = "You can only fix up the view engine collection if it hasn't already been modified (i.e., doesn't contain exactly two engines, one the Web Form view engine and one the Razor view engine.";
            
            var viewEngines = ViewEngines.Engines;
            if (viewEngines.Count != 2)
                throw new InvalidOperationException(errorMessage);

            var razorViewEngine = viewEngines.Where(x => x.GetType() == typeof(MvcRazorViewEngine)).SingleOrDefault();
            var webFormViewEngine = viewEngines.Where(x => x.GetType() == typeof(MvcWebFormViewEngine)).SingleOrDefault();

            if (razorViewEngine == null || webFormViewEngine == null)
                throw new InvalidOperationException(errorMessage);
            
            viewEngines.Clear();
            viewEngines.Add(new WebFormViewEngine());
            viewEngines.Add(new RazorViewEngine());
        }

        public static void UseActionInjection()
        {
            if (MvcDependencyResolver.Current == null)
                throw new InvalidOperationException("Action injection requires a dependency resolver, but DependencyResolver.Current is null.");

            var dependencyRegistry = MvcDependencyResolver.Current.GetService<IDependencyRegistry>();
            if (dependencyRegistry == null)
                throw new InvalidOperationException("Action injection requires a dependency registry, but DependencyResolver.Current.GetService<IDependencyRegistry>() returned null.");

            dependencyRegistry.RegisterCreator<IControllerActivator>(() => new ActionInjectionControllerActivator());
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

                dependencyResolver.RegisterCreator<IDependencyRegistry>(() => dependencyResolver);
                dependencyResolver.RegisterCreator<INamedDependencyResolver>(() => dependencyResolver);

                MvcDependencyResolver.SetResolver(dependencyResolver);
            }
        }

        public static void UseRouteRegistrars()
        {
            UseRouteRegistrars(new[] { Assembly.GetCallingAssembly() });
        }
        
        public static void UseRouteRegistrars(IEnumerable<Assembly> assembliesWithRouteRegistrars)
        {
            var registrarInterface = typeof(IRouteRegistrar);

            var registrars = assembliesWithRouteRegistrars
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type != registrarInterface && registrarInterface.IsAssignableFrom(type))
                .Select(type => Activator.CreateInstance(type) as IRouteRegistrar);

            foreach (var registrar in registrars)
                registrar.RegisterRoutes(RouteTable.Routes);
        }

        public static void UseScopedFilters()
        {
            if (MvcDependencyResolver.Current == null)
                throw new InvalidOperationException("Scoped filters require a dependency resolver, but DependencyResolver.Current is null.");

            var dependencyRegistry = MvcDependencyResolver.Current.GetService<IDependencyRegistry>();
            if (dependencyRegistry == null)
                throw new InvalidOperationException("Scoped filters require a dependency registry, but DependencyResolver.Current.GetService<IDependencyRegistry>() returned null.");

            dependencyRegistry.RegisterCreator<IFilterProvider>(() => new FilterProvider());
        }
    }
}