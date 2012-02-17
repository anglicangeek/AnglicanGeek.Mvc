0.3.1
---
* Extracted the dependency container and object creation bits into their own package.

0.3
---
* Added new feature: method parameter injection for controller actions
* Added new feature: simple dependency container now supports named dependencies
* Added INamedDependencyResolver for named dependency resolution
* The simple dependency container automatically registers iteself as an IDependencyRegistry and INamedDependencyResolver
* Modified scoped filters and action injection features to use the IDependencyRegistry interface so that they can be used with other dependency containers
* By default, only route registrars in the calling assembly are now used, unless a list of assemblies is passed to UseRouteRegistars
* Renamed the Bootstrapper in App_Start

0.4
---
* Changed action method parameter injection to work for all parameter types, not just abstract classes and interfaces. The action invoker will not first try to resolve a parameter against the current container, and if it doesn't exist in the container, then it will defer to the base impl.

0.4.1
---
* When the container implements IDependencyRegistrar, it will query the container for all parameter types during action injection, not just abstract classes and interfaces; if not, it will only try to resolve abstract classes and interfaces (fixes bug in 0.4).