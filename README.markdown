AnglicanGeek.Mvc is a library that has the stuff I use with every ASP.NET MVC app I craft. 

To install via NuGet:

    install-package AnglicanGeek.Mvc

AnglicanGeek.Mvx provides:

A Simple Dependency Container and Interface-Driven Dependency Registration
--------------------------------------------------------------------------

AnglicanGeek.Mvc includes a simple dependency container to use as a dependency resolver with MVC. You register dependencies via the [IDependencyRegistrar](https://github.com/anglicangeek/AnglicanGeek.Mvc/blob/master/Lib/IDependencyRegistrar.cs) interface, a simple dependency container will be registered with ASP.NET MVC, and your dependency registar will automatically be invoked during [application pre-start](https://github.com/anglicangeek/AnglicanGeek.Mvc/blob/master/Lib/PreApplicationStartCode.cs) interface. For example:

	public class MyDependencyRegistrar : IDependencyRegistrar
	{
		public void RegisterDependencies(IDependencyRegistry dependencyRegistry)
		{
			dependencyRegistry.RegisterConstructorParameterValue("name", () => GetValue());
			dependencyRegistry.RegisterBinding<IServiceInterface, ConcreteService>();
			dependencyRegistry.RegisterCreator<AbstractClass>(() => new ConcreteClass());
		}
	}

This simple container supports constructor injection, in three ways:

* You can register a binding from a contract type (usually an interface or an abstract class) to an implementing type; when a constructor parameter's type matches a binding, the container will create the bound, implementing type (recursively resolving constructor parameters)
* You can register a creator thunk to create an object for a constructor parameter by type; this can also be used to create singleton objects
* You can register a value thunk to get the value for a constructor parameter by name; this is useful for things such as connection strings or other settings

The simple container does not provide lifecycle management (ASP.NET MVC should do the right thing when your objects are disposable), and does not provide any other means for dependency injection (such as property injection).

You may have more than one dependency registrar in your app, but the order in which they are invoked is not deterministic. You can also use IDependencyRegistrar in your libraries for automatic dependency registration when used alongside AnglicanGeek.Mvc (for an example, see AnglicanGeek.Crypto).

To use the simple dependency container, add the following line to your application's start-up code:

    AnglicanGeek.Mvc.Configurator.UseSimpleDependencyContainer();

Method Injection for Controller Actions
---------------------------------------

AnglicanGeek.Mvc provides method parameter injection for controller actions. If a parameter value is null after model binding, a custom action invoker will attempt to resolve the parameter as a dependency using the registered dependency resolver. Only interfaces or abstract parameter types will be resolved as dependencies. For example:

	public ActionResult Index(
        int id,
		IPersonService personService)
    {
        var person = crudService.Read(id);
		return View(person);
    }

*Named Dependencies*

If the current dependency resolver provides a [named dependency resolver](https://github.com/anglicangeek/AnglicanGeek.Mvc/blob/master/Lib/INamedDependencyResolver.cs), you can use named dependencies for action injection. A named dependency will only be resolved if both the parameter's type and the parameter's name have been registered in the [dependenct registry](https://github.com/anglicangeek/AnglicanGeek.Mvc/blob/master/Lib/IDependencyRegistry.cs). 

To use action injection, add the following line to your application's start-up code:

    AnglicanGeek.Mvc.Configurator.UseActionInjection();

Interface-Driven Global Filters with Constructor Injection
----------------------------------------------------------

AnglicanGeek.Mvc includes a filter provider that looks for filter types that implement the [IScopedFilter](https://github.com/anglicangeek/AnglicanGeek.Mvc/blob/master/Lib/IScopedFilter.cs) interface. This interface allows you to specify the filter's order and scope (for instance, to create a global filter) and provides a method to inspect the context to determine if the filter should execute. For example:

	public class MyGlobalActionFilter : IActionFilter, IScopedFilter
	{
		public int? Order
		{
			get { return null; }
		}

		public FilterScope Scope
		{
			get { return FilterScope.Global; }
		}

		public bool AppliesToContext(
			ControllerContext controllerContext, 
			ActionDescriptor actionDescriptor)
		{
			return true; // always execute
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
			// do something
		}

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			// do something
		}
	}

Note that the IScopedFilter interface must be implemented on a type that *also* implements one of the four MVC filter interfaces: IActionFilter, IResultFilter, IExceptionFilter, or IAuthorizationFilter. If the IScopedFilter interface is implemented on a type without one of these, that type will be ignored.


To use scoped filters, add the following line to your application's start-up code:

    AnglicanGeek.Mvc.Configurator.UseScopedFilters();

Using scoped filters relies on a dependency resolver that provides a [dependency registry](https://github.com/anglicangeek/AnglicanGeek.Mvc/blob/master/Lib/IDependencyRegistry.cs). The simple dependency container included in AnglicanGeek.Mvc provides a dependency registry, and adding one to the container of your choice is likely easy to do.

Interface-Driven Route Registration
-----------------------------------

If, like me, you consider the presence of global.asax in an MVC app an eye-sore, you might like to use my [IRouteRegistrar](https://github.com/anglicangeek/AnglicanGeek.Mvc/blob/master/Lib/IRouteRegistrar.cs) interface for route registration. Here is an example:

	public class MyRouteRegistrar : IRouteRegistrar
	{
		public void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				"Default", 
				"{controller}/{action}/{id}", 
				new { controller = "Home", action = "Index", id = UrlParameter.Optional });
		}
	}

By default, only route registars in the calling assembly will be used, but you can also pass a list of assemblies containing route registars to use.

To use route registrars, add the following line to your application's start-up code:

    AnglicanGeek.Mvc.Configurator.UseRouteRegistrars();

A Flat Folder for View Templates
--------------------------------

I've never liked being forced to put my view templates into a folder that matches my controller's name. For a large app with lots of views, this organization might make sense, but for all a small app with one controller it's needless. So AnglicanGeek.Mvc will fix up the registered view engines *only when they haven't already been manipulated* to allow view template paths like:

* ~/views/_layout.cshtml
* ~/views/view.cshtml
* ~/views/_partial.cshtml

To support a flat folder for view engines, add the following line to your application's start-up code:

    AnglicanGeek.Mvc.Configurator.FixUpViewEngines();

