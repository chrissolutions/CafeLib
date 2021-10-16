using System;
using CafeLib.Core.Logging;

namespace CafeLib.Core.UnitTests.Logging
{
    public class TestLogWriter : LogEventWriter
    {
        public TestLogWriter() 
            : base("TestLogWriter", LogWriterListener)
        {
        }

        public TestLogWriter(string category, ILogEventReceiver receiver) 
            : base(category, receiver)
        {
        }

        private static void LogWriterListener(LogEventMessage logEventMessage)
        {
            Console.WriteLine("kilroy");
            //Assert.Equal("3: Log event message", logEventMessage.EventInfo.ToString());
            //Assert.Equal("Hello World", logEventMessage.Message);
        }
    }
}
