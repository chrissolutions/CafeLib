using System;
using CafeLib.Core.Extensions;
using CafeLib.Core.Logging;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.UnitTests.Logging
{
    public class TestLogSender : LogEventSender
    {
        public TestLogSender(string category, ILogEventReceiver receiver) 
            : base(category, receiver)
        {
        }

        public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter?.Invoke(state, exception);
            var messageInfo = state.GetType().IsAnonymousType() ? new LogMessageInfo(state.ToObjectMap()) : null;
            var eventMessage = new TestLogEventMessage(Category, this.ToErrorLevel(logLevel), new LogEventInfo(eventId), message, messageInfo, exception);
            Receiver.LogMessage(eventMessage);
        }
    }
}