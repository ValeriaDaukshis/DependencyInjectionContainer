using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DIContainer
{
    public class DependenciesConfiguration
    {
        private ConcurrentDictionary<Type, List<ImplementationInfo>> _dependencesList;
        public DependenciesConfiguration()
        {
            _dependencesList = new ConcurrentDictionary<Type, List<ImplementationInfo>>();
        }

        public void Register<TDependency, TImplementation>(bool singleTone = false)
        {
            Register(typeof(TDependency), typeof(TImplementation), singleTone);
        }
        
        public void Register(Type dependency, Type implementation, bool singleTone = false)
        {
            ImplementationInfo implementationInfo = new ImplementationInfo(implementation, singleTone);
            if (_dependencesList.ContainsKey(implementation))
            {
                _dependencesList[implementation].Add(implementationInfo);
            }
            else
            {
                _dependencesList.TryAdd(dependency, new List<ImplementationInfo>());
                _dependencesList[dependency].Add(implementationInfo);
            }
        }
    }
}