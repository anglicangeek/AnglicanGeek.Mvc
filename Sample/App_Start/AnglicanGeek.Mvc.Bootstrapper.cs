using System;
using AnglicanGeek.Mvc;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Sample.App_Start.AnglicanGeek.Mvc.Bootstrapper), "PreApplicationStart")]

namespace Sample.App_Start.AnglicanGeek.Mvc
{
    public static class Bootstrapper
    {
        public static void PreApplicationStart() 
        {
            // Comment or delete the parts of AnglicanGeek.Mvc you don't want to use, below.
            
            Configurator.UseRouteRegistrars();
            Configurator.UseSimpleDependencyContainer();
            Configurator.UseScopedFilters();
            Configurator.FixUpViewEngines();
        }
    }
}