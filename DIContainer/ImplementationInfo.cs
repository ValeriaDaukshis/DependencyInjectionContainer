using System;

namespace DIContainer
{
    public class ImplementationInfo
    {
        public Type implementationType { get; }
        public bool singleTone { get; }  
        
        public ImplementationInfo(Type t, bool singleTone)
        {
            this.singleTone = singleTone;
            this.implementationType = t;
        }
    }
}