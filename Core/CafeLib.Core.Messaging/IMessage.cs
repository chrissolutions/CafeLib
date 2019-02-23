using System;

namespace CafeLib.Core.Messaging
{
    public interface IMessage
    {
        DateTime TimeStamp { get; }
    }
}
