using System;

namespace DIContainer
{
    public sealed class SingleToneRealization
    {
        private static volatile object _syncRoot;
        private object _instance;
        private readonly Type _implementationType;

        public SingleToneRealization(Type implementationType)
        {
            this._implementationType = implementationType;
            _syncRoot = new object();
            _instance = null;
        }
        
        public object GetInstance(DependencyProvider provider)
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = provider.Create(_implementationType);
                    }
                }
            }
            return _instance;
        }
    }
}