using CafeLib.Core.Eventing;
using CafeLib.Core.IoC;

namespace CafeLib.Core.UnitTests.EventHosts
{
    public class FooEventHost
    {
        public FooEventHost(IServiceResolver resolver)
        {
            var eventService = resolver.Resolve<IEventService>();
            eventService.Subscribe<EventServiceTest.CommonEventMessage>(x => x.Visited());
        }
    }
}
