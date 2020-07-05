using System;

namespace CafeLib.Core.Logging
{
    internal class LogEventReceiver : ILogEventReceiver
    {
        private readonly Action<LogEventMessage> _listener;

        public LogEventReceiver()
            : this(null)
        {
        }

        public LogEventReceiver(Action<LogEventMessage> listener)
        {
            _listener = listener;
        }

        public void LogMessage<T>(T message) where T : LogEventMessage
        {
            _listener?.Invoke(message);
        }
    }
}