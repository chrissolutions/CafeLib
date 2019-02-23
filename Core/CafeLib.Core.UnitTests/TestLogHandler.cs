using System;
using CafeLib.Core.Extensions;
using CafeLib.Core.Logging;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.UnitTests
{
    public class TestLogHandler : LogHandler
    {
        public TestLogHandler(string category, ILogEventMessenger messenger) 
            : base(category, messenger)
        {
        }

        public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!state.GetType().IsAnonymousType()) return;
            var message = formatter?.Invoke(state, exception);
            var messageInfo = typeof(TState).ToObjectMap(state);
            var eventMesaage = new TestLogEventMessage(Category, this.ToErrorLevel(logLevel), new LogEventInfo(eventId), message, messageInfo, exception);
            Messenger?.LogMessage(eventMesaage);
        }
    }
}