using System;
using CafeLib.Core.Eventing;

namespace CafeLib.Web.SignalR
{
    public class SignalRAdviseMessage : EventMessage
    {
        public bool IsException => Exception != null;

        public string Message { get; }

        public Exception Exception { get; }

        internal SignalRAdviseMessage(string message, Exception ex = null)
        {
            Message = message;
            Exception = ex;
        }

        internal SignalRAdviseMessage(Exception ex)
        {
            Exception = ex;
        }
    }
}
