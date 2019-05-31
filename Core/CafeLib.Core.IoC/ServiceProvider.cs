using System;
using System.Net;
using System.Threading.Tasks;
using CafeLib.Core.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.IoC
{
    public static class ServiceProvider
    {
        public static IServiceRegistry CreateRegistry()
        {
            return new ServiceRegistry();
        }
    }
}
