using System;
using System.Collections.Generic;
using CafeLib.Core.Diagnostics;
using CafeLib.Core.Logging;

namespace CafeLib.Core.UnitTests.Logging
{
    public class TestLogEventMessage : LogEventMessage
    {
        public TestLogEventMessage(string category, ErrorLevel errorLevel, LogEventInfo eventInfo, string message, IDictionary<string, object> messageInfo, Exception exception = null)
            : base(category, errorLevel, eventInfo, message, exception)
        {
            MessageInfo = messageInfo;
        }

        public IDictionary<string, object> MessageInfo { get; }
    }
}