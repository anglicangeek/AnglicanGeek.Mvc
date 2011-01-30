AnglicanGeek.Mvc is a library that has stuff I use with *every* ASP.NET MVC app I craft. What it provides:

A Simple Dependency Container and Interface-Driven Dependency Registration
--------------------------------------------------------------------------

If you include a type in your app that implements the IDependencyRegistrar interface, a simple dependency container will be registered with ASP.NET MVC, and your dependency registar will automatically be invoked during application pre-start to push its dependencies into the container. For example:

public class MyDependencyRegistrar : IDependencyRegistrar
{
    public void RegisterDependencies(IDependencyRegistry dependencyRegistry)
    {
        dependencyRegistry.RegisterConstructorParameterValue("name", () => GetValue());
        dependencyRegistry.RegisterBinding<IServiceInterface, ConcreteService>();
        dependencyRegistry.RegisterCreator<AbstractClass>(() => new ConcreteClass());
    }
}

This simple dependency container supports constructor injection, in three ways:

* You can register a binding from a contract type (usually an interface or an abstract class) to an implementing type; when a constructor parameter's type matches a binding, the object creator will implicitly create it (recursively resolving dependencies)
* You can register a creator thunk to create an object for a constructor parameter by type; this can also be used to create singleton objects
* You can register a value thunk to get the value for a constructor parameter by name; this is useful for things such as connection strings or other settings

The simple dependency resolver does not provide lifecycle management (ASP.NET MVC does the right thing when your objects are disposable), and does not provide any other means for dependency injection (such as property injection).

You may have more than one dependency registrar, but the order in which they are called is not deterministic. You can also include them in other libraries for automatic dependency registration when used alongside AnglicanGeek.Mvc (for an example, see AnglicanGeek.Crypto).

Interface-Driven Global Filters with Constructor Injection
----------------------------------------------------------

AnglicanGeek.Mvc includes a filter provider that looks for filter types that implement the IScopedFilter interface. This interface allows you to specify the filter's order and scope (for instance, to create a global filter) and provides a method to inspect the context to determine if the filter should execute. For example:

public class MyGlobalActionFilter : IActionFilter, I ScopedFilter
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

This filter provider relies on a dependency resolver. If you're using AnglicanGeek.Mvc's simple dependency container, the filter provider is automatically pushed into the container, and you can use the IScopedFilter interface without any setup. If you're using some other dependency resolver, you'll need to make that resolver aware of the filter provider yourself. How you do that will depend on your dependency resolver.

Interface-Driven Route Registration
-----------------------------------

If, like me, you consider the presence of global.asax in an MVC app an eye-sore, you might like to use my IRouteRegistrar interface for route registration. Here is an example:

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

You can have as many IRouteRegistrar implementations in your app as you like, and they will all be invoked, but not in a deterministic order. You can also use the IRouteRegistrar interface in libraries, and when used along side AnglicanGeek.Mvc, the routes will automatically be registered.

A Flat Folder for View Templates
--------------------------------

I've never liked being forced to put my view templates into a folder that matches my controller's name. For a large app with lots of views, this organization might make sense, but for all a small app with one controller it's needless. So AnglicanGeek.Mvc will fix up the registered view engines *only when they haven't already been manipulated* to allow view template paths like:

* ~/views/_layout.cshtml
* ~/views/view.cshtml
* ~/views/_partial.cshtml