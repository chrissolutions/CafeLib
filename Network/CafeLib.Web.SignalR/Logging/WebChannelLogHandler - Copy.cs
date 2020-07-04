using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace CafeLib.Web.SignalR
{
    internal class WebChannelLogReceiver : LogHandler
    {
        /// <summary>
        /// Web channel log handler.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="messenger"></param>
        public WebChannelLogHandler(string category, ILogEventMessenger messenger) 
            : base(category, messenger)
        {
        }

        /// <summary>
        /// Log web channel message to the messenger.
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        /// <remarks>
        ///     THIS IS A HACK due to Microsoft using logging to send subsystem status.
        ///     This hack makes use of the undocumented LogValues type that happens to be an IEnumerable.
        ///     The danger is that Microsoft can alter the format of the log messages from the subsystem.
        /// </remarks>
        public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter?.Invoke(state, exception);
            var messageInfo = ((IEnumerable<KeyValuePair<string, object>>)state).ToDictionary(x => x.Key, x => x.Value);
            var eventMesaage = new WebChannelLogEventMessage(Category, this.ToErrorLevel(logLevel), new LogEventInfo(eventId), message, messageInfo, exception);
            Messenger?.LogMessage(eventMesaage);
        }
    }
}