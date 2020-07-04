using System;
using CafeLib.Core.Logging;
using CafeLib.Core.Support;

namespace CafeLib.Core.UnitTests.Logging
{
    public class TestLogEventMessage : LogEventMessage
    {
        public TestLogEventMessage(string category, ErrorLevel errorLevel, LogEventInfo eventInfo, string message, LogMessageInfo messageInfo, Exception exception = null)
            : base(category, errorLevel, eventInfo, message, messageInfo, exception)
        {
        }
    }
}