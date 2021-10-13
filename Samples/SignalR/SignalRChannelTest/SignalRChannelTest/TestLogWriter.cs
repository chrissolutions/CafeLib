using System;
using CafeLib.Core.Logging;
using CafeLib.Web.SignalR;

namespace SignalRChannelTest
{
    public class TestLogWriter : LogEventWriter<SignalRChannel>
    {
        public TestLogWriter()
            : base(LogWriterListener)
        {
        }

        private static void LogWriterListener(LogEventMessage m)
        {
            Console.WriteLine($"Logging: {m.LogLevel}, {m.Category}, {m.Message}");
        }
    }
}