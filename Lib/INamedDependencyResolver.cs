using System;

namespace AnglicanGeek.Mvc
{
    public interface INamedDependencyResolver
    {
        object GetService(Type serviceType, string name);
    }
}
