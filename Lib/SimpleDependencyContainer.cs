using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace AnglicanGeek.Mvc
{
    public class SimpleDependencyContainer : IDependencyRegistry, IDependencyResolver
    {
        readonly Dictionary<Type, IList<Func<object>>> registeredTypeCreators = new Dictionary<Type, IList<Func<object>>>();
        readonly Dictionary<string, Func<object>> registeredConstructorValues = new Dictionary<string, Func<object>>();

        object CreateInstance(Type type)
        {
            var args = new List<object>();

            // TODO: What to do if there are multiple constructors?
            ConstructorInfo[] constructors = type.GetConstructors();

            if (constructors.Length != 0)
            {
                foreach (ParameterInfo info in constructors[0].GetParameters())
                    if (registeredConstructorValues.ContainsKey(info.Name))
                        args.Add(registeredConstructorValues[info.Name]);
                    else
                        args.Add(GetService(info.ParameterType));
            }
            if (args.Count > 0)
                return Activator.CreateInstance(type, args.ToArray());

            return Activator.CreateInstance(type);
        }
        
        public object GetService(Type serviceType)
        {
            IList<Func<object>> creators;
            bool typeIsRegistered;

            typeIsRegistered = registeredTypeCreators.TryGetValue(serviceType, out creators);

            if (!typeIsRegistered)
                if (!serviceType.IsAbstract && !serviceType.IsInterface)
                    return CreateInstance(serviceType);
                else
                    return null;

            if (creators.Count > 1)
                throw new InvalidOperationException(string.Format("Cannot create the type '{0}' because it has more than one registered creators", serviceType));
            
            return creators[0]();
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            foreach (var key in registeredTypeCreators.Keys)
                if (key.IsAssignableFrom(serviceType))
                    foreach (var creator in registeredTypeCreators[key])
                        yield return creator();
        }

        public IEnumerable<T> GetServices<T>()
        {
            foreach (var key in registeredTypeCreators.Keys)
                if (key.IsAssignableFrom(typeof(T)))
                    foreach(var creator in registeredTypeCreators[key])
                        yield return (T)creator();
        }

        public void RegisterBinding(
            Type from, 
            Type to)
        {   
            if (!registeredTypeCreators.ContainsKey(from))
                    registeredTypeCreators.Add(from, new List<Func<object>> { () => CreateInstance(to) });
            else
                registeredTypeCreators[from].Add(() => CreateInstance(to));
        }

        public void RegisterBinding<TFrom, TTo>()
        {
            RegisterBinding(typeof(TFrom), typeof(TTo));
        }

        public void RegisterConstructorParameterValue(
            string name,
            Func<object> valueThunk)
        {
            // TODO: what to do if it already is registered?

            if (!registeredConstructorValues.ContainsKey(name))
            {
                lock (registeredConstructorValues)
                {
                    if (!registeredConstructorValues.ContainsKey(name))
                        registeredConstructorValues.Add(name, valueThunk);
                }
            }
        }

        public void RegisterCreator(
            Type type, 
            Func<object> creator)
        {
            if (!registeredTypeCreators.ContainsKey(type))
                registeredTypeCreators.Add(type, new List<Func<object>> { creator });
            else
                registeredTypeCreators[type].Add(creator);
        }

        public void RegisterCreator<T>(Func<object> creator)
        {
            RegisterCreator(typeof(T), creator);
        }
    }
}