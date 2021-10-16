using System;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace CafeLib.Core.Eventing
{
    public interface IEventBrokerService
    {
        IEventBroker CreateEventBroker(string name);
        IEventBroker GetEventBroker(string name);
    }
}
