using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DIContainer
{
    public class DependencyProvider
    {
        private DependenciesConfiguration _configuration;
        public DependencyProvider(DependenciesConfiguration configuration)
        {
            if(ValidateConfiguration(configuration))
            {
                this._configuration = configuration;
            }
        }
        
        private bool ValidateConfiguration(DependenciesConfiguration dependenciesConfiguration)
        {
            foreach (Type dependences in dependenciesConfiguration.DependencesList.Keys)
            {
                if (!dependences.IsValueType)
                {
                    foreach (ImplementationInfo dependency in dependenciesConfiguration.DependencesList[dependences])
                    {
                        Type implementation = dependency.ImplementationType;

                        if (implementation.IsAbstract || implementation.IsInterface || !dependences.IsAssignableFrom(implementation))
                        {
                            throw new ArgumentException("Config is not valid");
                        }
                    }
                }
                else
                {
                    throw new ArgumentException("Config is not valid");
                }
            }
            return true;
        }
        
        public T Resolve<T>() where T: class
        {
            Type t = typeof(T);

            return (T)Resolve(t);
        }
        
        private object Resolve(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return CreateGeneric(type);
            }
            
            _configuration.DependencesList.TryGetValue(type, out List<ImplementationInfo> implementations);
            if (implementations != null)
                return GetInstance(implementations.First());

            if (type.IsGenericType)
            {
                var genericDefinition = type.GetGenericTypeDefinition();                    
                _configuration.DependencesList.TryGetValue(genericDefinition, out implementations);
                if (implementations != null)
                    return GetInstance(implementations.First());
            }
            
            throw new ArgumentException("Unknown type " + type.Name);
            
        }
        public object Create(Type type)
        {
            return null;
        }

        private object CreateGeneric(Type type)
        {
            Type resolve = type.GetGenericArguments()[0];
            
            _configuration.DependencesList.TryGetValue(resolve,out List<ImplementationInfo> implementations);
            if (implementations != null)
            {
                var result = Activator.CreateInstance(typeof(List<>).MakeGenericType(resolve));
                foreach (ImplementationInfo implementation in implementations)
                {
                    ((IList)result).Add(GetInstance(implementation));
                }
                return result;
            }
            
            throw new ArgumentException("Unknown type "+type.Name);
        }
        
        private object GetInstance(ImplementationInfo implementation)
        {
            if (implementation.SingleTone)
            {
                return implementation.SingleToneRealization.GetInstance(this);
            }
            
            return Create(implementation.ImplementationType);
        }
    }
}