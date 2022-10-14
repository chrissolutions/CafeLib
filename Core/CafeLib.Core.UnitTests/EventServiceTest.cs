using System;
using System.Threading.Tasks;
using CafeLib.Core.Eventing;
using CafeLib.Core.IoC;
using CafeLib.Core.UnitTests.EventHosts;
using Xunit;
// ReSharper disable NotAccessedField.Local

namespace CafeLib.Core.UnitTests
{
    public class EventServiceTest : IDisposable
    {
        protected IServiceResolver Resolver;

        private int _commonEventMessageVisits;
        private readonly FooEventHost _fooHost;
        private readonly BarEventHost _barHost;
        private readonly FooEventHostAsync _fooHostAsync;
        private readonly BarEventHostAsync _barHostAsync;

        public EventServiceTest()
        {
            Resolver = IocFactory.CreateRegistry()
                .AddSingleton<IEventService>(_ => new EventService())
                .GetResolver();

            _fooHost = new FooEventHost(Resolver);
            _barHost = new BarEventHost(Resolver);
            _fooHostAsync = new FooEventHostAsync(Resolver);
            _barHostAsync = new BarEventHostAsync(Resolver);
        }

        public void Dispose()
        {
            Resolver.Dispose();
        }

        [Fact]
        public void CommonEventMessage_Test()
        {
            var eventService = Resolver.Resolve<IEventService>();
            eventService.Publish(new CommonEventMessage(this));
            Assert.Equal(2, _commonEventMessageVisits);
        }

        [Fact]
        public async Task CommonEventMessageAsync_Test()
        {
            ClearCommonVisits();
            var eventService = Resolver.Resolve<IEventService>();
            await eventService.PublishAsync(new CommonEventMessage(this));
            Assert.Equal(2, _commonEventMessageVisits);
        }

        private void AddCommonVisits()
        {
            _commonEventMessageVisits += 1;
        }

        private void ClearCommonVisits()
        {
            _commonEventMessageVisits = 0;
        }

        public class CommonEventMessage : EventMessage
        {
            private readonly EventServiceTest _test;

            public CommonEventMessage(EventServiceTest test)
            {
                _test = test;
            }

            public void Visited()
            {
                _test.AddCommonVisits();
            }

            public Task VisitedAsync()
            {
                _test.AddCommonVisits();
                return Task.CompletedTask;
            }
        }

        public class CommonEventMessageAsync : CommonEventMessage
        {
            public CommonEventMessageAsync(EventServiceTest test)
                : base(test)
            {
            }
        }
    }
}
