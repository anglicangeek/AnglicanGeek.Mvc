using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MvcDependencyResolver = System.Web.Mvc.DependencyResolver;
using MvcRazorViewEngine = System.Web.Mvc.RazorViewEngine;
using MvcWebFormViewEngine = System.Web.Mvc.WebFormViewEngine;

[assembly: PreApplicationStartMethod(typeof(AnglicanGeek.Mvc.PreApplicationStartCode), "Start")]

namespace AnglicanGeek.Mvc
{
    public static class PreApplicationStartCode
    {
        static bool startWasCalled;
        
        public static void Start()
        {
            if (startWasCalled) 
                return;

            startWasCalled = true;

            RegisterDependencies();
            RegisterRoutes();
            FixUpViewEngines();
        }

        static void RegisterDependencies()
        {
            var dependencyResolver = new SimpleDependencyContainer();

            dependencyResolver.RegisterCreator<IFilterProvider>(() => new FilterProvider(dependencyResolver));

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

        static void RegisterRoutes()
        {
            var registrarInterface = typeof(IRouteRegistrar);

            var registrars = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type != registrarInterface && registrarInterface.IsAssignableFrom(type))
                .Select(type => Activator.CreateInstance(type) as IRouteRegistrar);

            foreach (var registrar in registrars)
                registrar.RegisterRoutes(RouteTable.Routes);
        }

        static void FixUpViewEngines()
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
    }
}