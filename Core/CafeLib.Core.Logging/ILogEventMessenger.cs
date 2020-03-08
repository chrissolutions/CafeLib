namespace CafeLib.Core.Logging
{
    /// <summary>
    /// Logs event by classification.
    /// </summary>
    public interface ILogEventMessenger
    {
        /// <summary>
        /// Log message receiver.
        /// </summary>
        /// <typeparam name="T">LogEventMessage type</typeparam>
        /// <param name="message">event message</param>
        void LogMessage<T>(T message) where T : LogEventMessage;
    }
}
