using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CafeLib.Core.Diagnostics;
using CafeLib.Core.Logging;

namespace CafeLib.Core.Client.SignalR
{
    public class WebChannelLogEventMessage : LogEventMessage
    {
        public WebChannelLogEventMessage(string category, ErrorLevel errorLevel, LogEventInfo eventInfo, string message, IDictionary<string, object> messageInfo, Exception exception = null)
            : base(category, errorLevel, eventInfo, message, exception)
        {
            MessageInfo = new ReadOnlyDictionary<string,object>(messageInfo);
        }

        public IReadOnlyDictionary<string, object> MessageInfo { get; }
    }
}