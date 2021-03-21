// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Core.Eventing
{
    public interface IEventBroker : IEventService
    {
        string Name { get; }
    }
}
