using System;
using CafeLib.Core.Logging;

namespace CafeLib.Core.UnitTests.Logging
{
    public class TestLogReceiver : ILogEventReceiver
    {
        private readonly Action<LogEventMessage> _listener;

        public TestLogReceiver(Action<LogEventMessage> listener)
        {
            _listener = listener;
        }

        public void LogMessage<T>(T message) where T : LogEventMessage
        {
            _listener?.Invoke(message);
        }
    }
}