// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Core.Eventing
{
    public interface IEventService : IEventBroker
    {
        IEventBroker CreateBroker(string name);
        IEventBroker GetBroker(string name);
    }
}
