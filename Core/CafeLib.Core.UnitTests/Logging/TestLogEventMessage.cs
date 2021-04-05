using System;
using CafeLib.Core.Logging;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.UnitTests.Logging
{
    public class TestLogEventMessage : LogEventMessage
    {
        public TestLogEventMessage(string category, LogLevel logLevel, LogEventInfo eventInfo, string message, LogMessageInfo messageInfo, Exception exception = null)
            : base(category, logLevel, eventInfo, message, messageInfo, exception)
        {
        }
    }
}