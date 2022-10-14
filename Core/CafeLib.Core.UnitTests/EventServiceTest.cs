using System;
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

        public EventServiceTest()
        {
            Resolver = IocFactory.CreateRegistry()
                .AddSingleton<IEventService>(_ => new EventService())
                .GetResolver();

            _fooHost = new FooEventHost(Resolver);
            _barHost = new BarEventHost(Resolver);
        }

        public void Dispose()
        {
            Resolver.Dispose();
        }

        [Fact]
        public void CommonEventMessageTest()
        {
            var eventService = Resolver.Resolve<IEventService>();
            eventService.Publish(new CommonEventMessage(this));
            Assert.Equal(2, _commonEventMessageVisits);
        }

        private void AddCommonVisits()
        {
            _commonEventMessageVisits += 1;
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
        }
    }
}
