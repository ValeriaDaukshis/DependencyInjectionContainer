using System;

namespace DIContainer
{
    public class ImplementationInfo
    {
        public Type ImplementationType { get; }
        public bool SingleTone { get; }
        public SingleToneRealization SingleToneRealization { get; }
        
        public ImplementationInfo(Type t, bool singleTone)
        {
            this.SingleTone = singleTone;
            this.ImplementationType = t;
            SingleToneRealization = new SingleToneRealization(this.ImplementationType);
        }
    }
}