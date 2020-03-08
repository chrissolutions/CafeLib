using System;

namespace CafeLib.Core.Eventing
{
    public interface IEventMessage
    {
        /// <summary>
        /// Event message Id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Event message sender
        /// </summary>
        object Sender { get; }

        /// <summary>
        /// Event sender timestamp.
        /// </summary>
        DateTime TimeStamp { get; }
    }
}
