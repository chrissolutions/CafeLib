using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CafeLib.Core.Logging;
using CafeLib.Core.Support;

namespace CafeLib.Web.SignalR.Logging
{
    public class SignalRLogEventMessage : LogEventMessage
    {
        public SignalRLogEventMessage(string category, ErrorLevel errorLevel, LogEventInfo eventInfo, string message, IDictionary<string, object> messageInfo, Exception exception = null)
            : base(category, errorLevel, eventInfo, message, exception)
        {
            MessageInfo = new ReadOnlyDictionary<string,object>(messageInfo);
        }

        public IReadOnlyDictionary<string, object> MessageInfo { get; }
    }
}