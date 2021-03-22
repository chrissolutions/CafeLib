using System;
using CafeLib.Core.Eventing;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Runnable
{
    public class RunnerEventMessage : EventMessage
    {
        public string Message { get; }

        public Exception Exception { get; }

        public RunnerEventMessage(string message)
            : this(message, null)
        {
        }

        public RunnerEventMessage(string message, Exception ex)
        {
            Message = message ?? string.Empty;
            Exception = ex;
        }

        public RunnerEventMessage(IEventMessage eventMessage)
            : base(eventMessage)
        {
            var msg = eventMessage as RunnerEventMessage ?? throw new InvalidCastException(nameof(eventMessage));
            Message = msg.Message;
            Exception = msg.Exception;
        }
    }
}