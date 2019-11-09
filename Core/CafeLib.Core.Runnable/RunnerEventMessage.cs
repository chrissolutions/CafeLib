using System;
using CafeLib.Core.Diagnostics;
using CafeLib.Core.Eventing;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Runnable
{
    public class RunnerEventMessage : EventMessage
    {
        public string Message { get; }

        public Exception Exception { get; }

        public ErrorLevel ErrorLevel { get; set; }

        public RunnerEventMessage()
        {
            ErrorLevel = ErrorLevel.Info;
        }

        public RunnerEventMessage(string message)
            : this(ErrorLevel.Info, message)
        {
        }

        public RunnerEventMessage(Exception ex)
            : this(ErrorLevel.Error, null, ex)
        {
        }

        public RunnerEventMessage(ErrorLevel errorLevel, string message, Exception exception = null)
        {
            ErrorLevel = errorLevel;
            Message = message ?? exception?.Message ?? string.Empty;
            Exception = exception;
        }

        public RunnerEventMessage(IEventMessage eventMessage)
            : base(eventMessage)
        {
            if (!(eventMessage is RunnerEventMessage message)) return;
            ErrorLevel = message.ErrorLevel;
            Message = message.Message ?? string.Empty;
            Exception = message.Exception;
        }
    }
}
