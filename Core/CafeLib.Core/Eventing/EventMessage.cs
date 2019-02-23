using System;

namespace CafeLib.Core.Eventing
{
    public abstract class EventMessage : IEventMessage
    {
        #region Automatic Properties

        /// <summary>
        /// Message id.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Message timestamp.
        /// </summary>
        public DateTime TimeStamp { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Message default constructor.
        /// </summary>
        protected EventMessage()
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Message constructor
        /// </summary>
        /// <param name="id">guid id</param>
        /// <param name="dateTime">timestamp</param>
        protected EventMessage(Guid id, DateTime dateTime)
        {
            Id = id;
            TimeStamp = dateTime;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="message">message</param>
        protected EventMessage(IEventMessage message)
        {
            Id = message.Id;
            TimeStamp = message.TimeStamp;
        }

        #endregion
    }
}
