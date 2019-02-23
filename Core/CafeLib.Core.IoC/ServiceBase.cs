using System;

namespace CafeLib.Core.IoC
{
    public abstract class ServiceBase : IServiceProvider
    {
        public virtual object GetService(Type serviceType)
        {
            return this;
        }
    }
}
