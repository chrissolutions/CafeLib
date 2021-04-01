using System;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Eventing
{
    public abstract class EventMessage : IEventMessage
    {
        private static readonly object Empty = new object();

        #region Automatic Properties

        /// <summary>
        /// Message id.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Event message sender;
        /// </summary>
        public object Sender { get; }

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
            Sender = new object();
            TimeStamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Message default constructor.
        /// </summary>
        protected EventMessage(object sender)
            : this()
        {
            Sender = sender ?? Empty;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="message">message</param>
        protected EventMessage(IEventMessage message)
        {
            Id = message.Id;
            Sender = message.Sender;
            TimeStamp = message.TimeStamp;
        }

        #endregion
    }
}
