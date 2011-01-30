using System;

namespace AnglicanGeek.Mvc
{
    public interface IDependencyRegistry
    {
        void RegisterBinding(
            Type from, 
            Type to);

        void RegisterBinding<TFrom, TTo>();
        
        void RegisterConstructorParameterValue(
            string name, 
            Func<object> value);
        
        void RegisterCreator(
            Type type, 
            Func<object> creator);
        
        void RegisterCreator<T>(Func<object> creator);
    }
}
