namespace CafeLib.Core.Logging
{
    /// <summary>
    /// Logs event by classification.
    /// </summary>
    public struct NullMessenger : ILogEventMessenger
    {
        /// <summary>
        /// Log OK event
        /// </summary>
        /// <param name="message">event message</param>
        public void LogMessage<T>(T message) where T : LogEventMessage
        {
            // do nothing.
        }
    }
}
