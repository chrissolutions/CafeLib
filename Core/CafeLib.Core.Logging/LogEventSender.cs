namespace CafeLib.Core.Logging
{
    public class LogEventSender : LoggerBase
    {
        #region Constructors

        /// <summary>
        /// LoggerHandler constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="receiver">logger receiver</param>
        public LogEventSender(string category, ILogEventReceiver receiver)
            : base(category, receiver)
        {
        }

        #endregion
    }
}
