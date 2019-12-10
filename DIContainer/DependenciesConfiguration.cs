using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DIContainer
{
    public class DependenciesConfiguration
    {
        public ConcurrentDictionary<Type, List<ImplementationInfo>> DependencesList { get; set; }
        public DependenciesConfiguration()
        {
            DependencesList = new ConcurrentDictionary<Type, List<ImplementationInfo>>();
        }

        public void Register<TDependency, TImplementation>(bool singleTone = false)
        {
            Register(typeof(TDependency), typeof(TImplementation), singleTone);
        }
        
        public void Register(Type dependency, Type implementation, bool singleTone = false)
        {
            ImplementationInfo implementationInfo = new ImplementationInfo(implementation, singleTone);
            if (DependencesList.ContainsKey(implementation))
            {
                DependencesList[implementation].Add(implementationInfo);
            }
            else
            {
                DependencesList.TryAdd(dependency, new List<ImplementationInfo>());
                DependencesList[dependency].Add(implementationInfo);
            }
        }
    }
}