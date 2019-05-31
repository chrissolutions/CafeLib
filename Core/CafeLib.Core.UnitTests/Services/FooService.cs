using System;
using CafeLib.Core.IoC;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.UnitTests.Services
{
    public interface IFooService : IServiceProvider
    {
        void DoThing(int number);
    }

    public class FooService : ServiceBase, IFooService
    {
        private readonly ILogger<FooService> _logger;
        public FooService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<FooService>();
        }

        public void DoThing(int number)
        {
            _logger.LogInformation($"Doing the thing {number}");
        }
    }
}