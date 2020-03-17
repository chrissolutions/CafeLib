using System;

namespace CafeLib.Core.Logging
{
    public abstract class LogAdapter : LoggerBase
    {
        #region LogReceiver
        private class LogReceiver : ILogEventReceiver
        {
            private readonly Action<LogEventMessage> _listener;

            public LogReceiver(Action<LogEventMessage> listener)
            {
                _listener = listener;
            }

            public void LogMessage<T>(T message) where T : LogEventMessage
            {
                _listener.Invoke(message);
            }
        }
        #endregion

        #region Variables

        private readonly Action<LogEventMessage> _listener;

        #endregion

        #region Constructors

        protected LogAdapter(string category, Action<LogEventMessage> listener)
            : base(category, null)

        {
            _listener = listener;
            Receiver = new LogReceiver(LogHandler);
        }


        #endregion

        #region Methods

        private void LogHandler(LogEventMessage message)
        {
            _listener.Invoke(message);
        }

        #endregion
    }
}
