namespace CafeLib.Core.Logging
{
    internal class LoggerCore : LoggerBase
    {
        #region Constructors

        /// <summary>
        /// LoggerHandler constructor.
        /// </summary>
        /// <param name="category">log category</param>
        /// <param name="receiver">logger receiver</param>
        public LoggerCore(string category, ILogEventReceiver receiver)
            : base(category, receiver)
        {
        }

        #endregion
    }
}