using System;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.Logging
{
    /// <summary>
    /// LogEventInfo
    /// </summary>
    public struct LogEventInfo : IEquatable<LogEventInfo>
    {
        #region Properties

        public static LogEventInfo Empty = new LogEventInfo();

        public int Id { get; }

        public string Name { get; }

        #endregion

        #region Constructors

        public LogEventInfo(int id, string name = null)
        {
            Id = id;
            Name = name;
        }

        public LogEventInfo(string content)
        {
            var index = content.IndexOf(':');
            var id = content.Substring(0, index);
            var name = content.Substring(index + 1);
            Id = int.Parse(id.Trim());
            Name = name.Trim();
        }

        public LogEventInfo(EventId eventId)
        {
            Id = eventId.Id;
            Name = eventId.Name;
        }

        #endregion

        #region Methods

        public override bool Equals(object obj)
        {
            try
            {
                return obj != null && Equals((LogEventInfo)obj);
            }
            catch
            {
                return false;
            }
        }

        public bool Equals(LogEventInfo other)
        {
            return Id == other.Id && string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{Id}: {Name}";
        }

        internal static EventId ToEventId(LogEventInfo eventInfo)
        {
            return new EventId(eventInfo.Id, eventInfo.Name);
        }

        #endregion 

        #region Operators

        public static implicit operator LogEventInfo(int id)
        {
            return new LogEventInfo(id);
        }

        #endregion
    }
}
