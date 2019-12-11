using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DIContainer
{
    public class DependencyProvider
    {
        private readonly DependenciesConfiguration _configuration;
        private readonly ConcurrentStack<Type> _stack;
        private Type _currentGenericType;
        public DependencyProvider(DependenciesConfiguration configuration)
        {
            if(ValidateConfiguration(configuration))
            {
                this._configuration = configuration;
                _stack = new ConcurrentStack<Type>();
            }
        }
        
        private bool ValidateConfiguration(DependenciesConfiguration dependenciesConfiguration)
        {
            foreach (Type dependences in dependenciesConfiguration.DependencesList.Keys)
            {
                if (dependences.IsValueType)
                {
                    throw new ArgumentException("Config is not valid");
                }
                
                foreach (ImplementationInfo dependency in dependenciesConfiguration.DependencesList[dependences])
                { 
                    Type implementation = dependency.ImplementationType;
                    if (implementation.IsAbstract || implementation.IsInterface ||
                        !dependences.IsAssignableFrom(implementation))
                    {
                        throw new ArgumentException("Config is not valid");
                    }
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
            {
                return GetInstance(implementations.First());
            }

            if (type.IsGenericType)
            {
                _currentGenericType = type;
                var genericDefinition = type.GetGenericTypeDefinition();                    
                _configuration.DependencesList.TryGetValue(genericDefinition, out implementations);
                if (implementations != null)
                {
                    return GetInstance(implementations.First());
                }
            }
            
            throw new ArgumentException("Unknown type " + type.Name);
            
        }
        public object Create(Type type)
        {
            object result;
            if (!_stack.Contains(type))
            {
                _stack.Push(type);

                if (type.IsGenericTypeDefinition)
                {
                    type = type.MakeGenericType(_currentGenericType.GenericTypeArguments);
                }

                ConstructorInfo constructor = GetConstructor(type);

                if (constructor != null)
                {
                    result = constructor.Invoke(GetConstructorParametersValues(constructor.GetParameters()));
                }
                else
                {
                    throw new ArgumentException("Can not find constructor!");
                }
                _stack.TryPop(out type);
            }
            else
            {
                result = null;
                _stack.Clear();
            }

            return result;
        }

        private ConstructorInfo GetConstructor(Type t)
        {
            ConstructorInfo result = null;
            ConstructorInfo[] constructors = t.GetConstructors();
            bool isRight;

            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();

                isRight = true;
                foreach (ParameterInfo parameter in parameters)
                {
                    if (!_configuration.DependencesList.ContainsKey(parameter.ParameterType))
                    {
                        isRight = false;
                        break;
                    }
                }

                if (isRight)
                {
                    result = constructor;
                    break;
                }
            }
            return result;
        }
        
        private object[] GetConstructorParametersValues(ParameterInfo[] parameters)
        {
            object[] result = new object[parameters.Length];

            for (int i=0; i<parameters.Length; i++)
            {
                result[i] = Resolve(parameters[i].ParameterType);
            }
            return result;
        }

        private object CreateGeneric(Type type)
        {
            Type resolve = type.GetGenericArguments()[0];
            
            _configuration.DependencesList.TryGetValue(resolve,out List<ImplementationInfo> implementations);
            if(implementations is null)
            {
                throw new ArgumentNullException(nameof(implementations));
            }
            
            var result = Activator.CreateInstance(typeof(List<>).MakeGenericType(resolve));
            foreach (ImplementationInfo implementation in implementations)
            {
                ((IList)result).Add(GetInstance(implementation));
            }
            return result;
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