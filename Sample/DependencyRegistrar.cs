
using AnglicanGeek.Mvc;

namespace Sample
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void RegisterDependencies(IDependencyRegistry dependencyRegistry)
        {
            dependencyRegistry.RegisterBinding<IGreetingService, GreetingService>();
        }
    }
}