using Microsoft.Extensions.Logging;

namespace CafeLib.Core.UnitTests.Services
{
    public interface IFooService
    {
        void DoThing(int number);
    }

    public class FooService : IFooService
    {
        private readonly ILogger _logger;
        public FooService(ILogger logger)
        {
            _logger = logger;
        }

        public void DoThing(int number)
        {
            _logger.LogInformation($"Doing the thing {number}");
        }
    }
}