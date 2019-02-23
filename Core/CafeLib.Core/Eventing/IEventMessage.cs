using System;

namespace CafeLib.Core.Eventing
{
    public interface IEventMessage
    {
        Guid Id { get; }
        DateTime TimeStamp { get; }
    }
}
