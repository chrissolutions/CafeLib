using CafeLib.Core.IoC;
using CafeLib.Core.UnitTests.Services;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class IoCTests
    {
        [Fact]
        public void IocTest()
        {
            var registry = ServiceProvider.CreateRegistry();
            registry
                .AddLogging(builder => builder.AddConsole().AddDebug())
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IBarService, BarService>();

            //do the actual work here
            var bar = registry.Resolve<IBarService>();
            bar.DoSomeRealWork();
        }
    }
}
