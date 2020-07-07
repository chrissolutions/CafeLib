using System;
using CafeLib.Core.Eventing;

namespace CafeLib.Web.SignalR
{
    public class SignalRAdviseMessage : EventMessage
    {
        public Exception Exception { get; }

        internal SignalRAdviseMessage(Exception ex)
        {
            Exception = ex;
        }
    }
}
