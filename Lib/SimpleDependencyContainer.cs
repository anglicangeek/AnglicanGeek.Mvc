using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace AnglicanGeek.Mvc
{
    public class SimpleDependencyContainer : IDependencyRegistry, IDependencyResolver, INamedDependencyResolver
    {
        readonly Dictionary<Tuple<Type, string>, IList<Func<object>>> registeredTypeCreators = new Dictionary<Tuple<Type, string>, IList<Func<object>>>();
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

            var key = new Tuple<Type, string>(serviceType, null);

            typeIsRegistered = registeredTypeCreators.TryGetValue(key, out creators);

            if (!typeIsRegistered)
                if (!serviceType.IsAbstract && !serviceType.IsInterface)
                    return CreateInstance(serviceType);
                else
                    return null;

            if (creators.Count > 1)
                throw new InvalidOperationException(string.Format("Cannot create the type '{0}' because it has more than one registered creators", serviceType));
            
            return creators[0]();
        }

        public object GetService(
            Type serviceType,
            string name)
        {
            IList<Func<object>> namedCreators;
            IList<Func<object>> typedCreators;
            bool nameIsRegistered;
            bool typeIsRegistered;

            var namedKey = new Tuple<Type, string>(serviceType, name);
            var typedKey = new Tuple<Type, string>(serviceType, null);

            nameIsRegistered = registeredTypeCreators.TryGetValue(namedKey, out namedCreators);
            typeIsRegistered = registeredTypeCreators.TryGetValue(typedKey, out typedCreators);

            Func<IList<Func<object>>, object> createInstance = new Func<IList<Func<object>>, object>((creators) =>
            {
                if (creators.Count > 1)
                    throw new InvalidOperationException(string.Format("Cannot create the type '{0}' because it has more than one registered creators", serviceType));

                return creators[0]();
            });

            if (nameIsRegistered)
            {
                return createInstance(namedCreators);
            }
            else if (typeIsRegistered)
            {
                return createInstance(typedCreators);
            }
            else
            {
                if (!serviceType.IsAbstract && !serviceType.IsInterface)
                    return CreateInstance(serviceType);
                else
                    return null;
            }
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public T GetService<T>(string name)
        {
            return (T)GetService(typeof(T), name);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            foreach (var key in registeredTypeCreators.Keys)
                if (key.Item1.IsAssignableFrom(serviceType))
                    foreach (var creator in registeredTypeCreators[key])
                        yield return creator();
        }

        public IEnumerable<T> GetServices<T>()
        {
            foreach (var key in registeredTypeCreators.Keys)
                if (key.Item1.IsAssignableFrom(typeof(T)))
                    foreach(var creator in registeredTypeCreators[key])
                        yield return (T)creator();
        }

        public void RegisterBinding(
            Type from, 
            Type to)
        {
            var key = new Tuple<Type, string>(from, null);
            
            if (!registeredTypeCreators.ContainsKey(key))
                    registeredTypeCreators.Add(key, new List<Func<object>> { () => CreateInstance(to) });
            else
                registeredTypeCreators[key].Add(() => CreateInstance(to));
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
            var key = new Tuple<Type, string>(type, null);
            
            if (!registeredTypeCreators.ContainsKey(key))
                registeredTypeCreators.Add(key, new List<Func<object>> { creator });
            else
                registeredTypeCreators[key].Add(creator);
        }

        public void RegisterCreator(
            Type type,
            string name,
            Func<object> creator)
        {
            var key = new Tuple<Type, string>(type, name);

            if (!registeredTypeCreators.ContainsKey(key))
                registeredTypeCreators.Add(key, new List<Func<object>> { creator });
            else
                registeredTypeCreators[key].Add(creator);
        }

        public void RegisterCreator<T>(Func<object> creator)
        {
            RegisterCreator(typeof(T), creator);
        }

        public void RegisterCreator<T>(
            string name,
            Func<object> creator)
        {
            RegisterCreator(typeof(T), name, creator);
        }
    }
}