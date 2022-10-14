using CafeLib.Core.Eventing;
using CafeLib.Core.IoC;

namespace CafeLib.Core.UnitTests.EventHosts
{
    public class FooEventHostAsync
    {
        public FooEventHostAsync(IServiceResolver resolver)
        {
            var eventService = resolver.Resolve<IEventService>();
            eventService.SubscribeAsync<EventServiceTest.CommonEventMessageAsync>(async x => await x.VisitedAsync());
        }
    }
}
