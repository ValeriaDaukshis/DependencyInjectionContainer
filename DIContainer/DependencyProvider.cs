using System;
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
            else
            {
                throw new ArgumentException("Config is not valid");
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
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
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
            return null;
        }
        
        private object GetInstance(ImplementationInfo tImplementation)
        {
            if (tImplementation.SingleTone)
            {
                return tImplementation.SingleToneRealization.GetInstance(this);
            }
            
            return Create(tImplementation.ImplementationType);
        }
    }
}