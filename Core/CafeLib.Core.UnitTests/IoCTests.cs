using CafeLib.Core.Caching;
using CafeLib.Core.IoC;
using CafeLib.Core.UnitTests.Logging;
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
            IDisposableService disposableService;
            using (var resolver = IocFactory.CreateRegistry()
                .AddSingleton<ILoggerFactory, TestLogFactory>()
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IBarService, BarService>()
                .AddSingleton<IDisposableService, DisposableService>()
                .GetResolver())
            {
                //do the actual work here
                var bar = resolver.Resolve<IBarService>();
                bar.DoSomeRealWork();
                disposableService = resolver.Resolve<IDisposableService>();
            }

            Assert.True(disposableService.IsDisposed);
        }

        [Fact]
        public void IocTestUsingResolutionType()
        {
            IDisposableService disposableService;
            using (var resolver = IocFactory.CreateRegistry()
                .AddSingleton<ILoggerFactory, TestLogFactory>()
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IBarService, BarService>()
                .AddSingleton<IDisposableService, DisposableService>()
                .GetResolver())
            {
                //do the actual work here
                var bar = (IBarService)resolver.Resolve(typeof(IBarService));
                bar.DoSomeRealWork();
                disposableService = resolver.Resolve<IDisposableService>();
            }

            Assert.True(disposableService.IsDisposed);
        }

        [Fact]
        public void ServiceProviderTest()
        {
            var resolver = IocFactory.CreateRegistry()
                .AddSingleton<IDictionaryService>(x => DictionaryService.Current)
                .AddSingleton<ITestService>(x => new TestService())
                .GetResolver();

            var propertyService = resolver.Resolve<IDictionaryService>();
            Assert.NotNull(propertyService);

            propertyService.SetEntry("name", "Kilroy");
            Assert.Equal("Kilroy", propertyService.GetEntry<string>("name"));

            var testService = resolver.Resolve<ITestService>();
            Assert.Equal("Kilroy is here!", testService.Test());
        }

        //[Fact]
        //public void ReplaceServiceAfterResolveTest()
        //{
        //    var registry = IocFactory.CreateRegistry()
        //        .AddPropertyService()
        //        .AddSingleton<ITestService, TestService>();

        //    // Setup test service.
        //    var resolver = registry.GetResolver();
        //    var testService = resolver.Resolve<ITestService>();
        //    Assert.Equal("Kilroy is here!", testService.Test());

        //    // Replace TestService with TestService2.
        //    registry.AddSingleton<ITestService, TestService2>();
        //    var testService2 = resolver.Resolve<ITestService>();
        //    Assert.Equal("Eagle has landed!", testService2.Test());
        //}
    }
}
