using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Support
{
    /// <summary>
    ///	Sequential guid.
    /// </summary>
    public class SequentialGuid
    {
        #region Private Variables

        private const int NumberOfBytes = 6;
        private const int PermutationsOfAByte = 256;
        private readonly long _maximumPermutations = (long)Math.Pow(PermutationsOfAByte, NumberOfBytes);
        private long _lastSequence;

        #endregion

        #region Automatic Properties

        public DateTime SequenceStartDate { get; }
        public DateTime SequenceEndDate { get; }

        #endregion

        public SequentialGuid(DateTime sequenceStartDate, DateTime sequenceEndDate)
        {
            SequenceStartDate = sequenceStartDate;
            SequenceEndDate = sequenceEndDate;
        }

        public SequentialGuid()
            : this(new DateTime(2011, 10, 15), new DateTime(2100, 1, 1))
        {
        }

        private static readonly Lazy<SequentialGuid> InstanceField = new Lazy<SequentialGuid>(() => new SequentialGuid());

        internal static SequentialGuid Instance => InstanceField.Value;

        public static Guid NewGuid()
        {
            return Instance.GetGuid();
        }

        public TimeSpan TimePerSequence
        {
            get
            {
                var ticksPerSequence = TotalPeriod.Ticks / _maximumPermutations;
                var result = new TimeSpan(ticksPerSequence);
                return result;
            }
        }

        public TimeSpan TotalPeriod
        {
            get
            {
                var result = SequenceEndDate - SequenceStartDate;
                return result;
            }
        }

        private long GetCurrentSequence(DateTime value)
        {
            var ticksUntilNow = value.Ticks - SequenceStartDate.Ticks;
            var result = ((decimal)ticksUntilNow / TotalPeriod.Ticks * _maximumPermutations - 1);
            return (long)result;
        }

        public Guid GetGuid()
        {
            return GetGuid(DateTime.Now);
        }

        private readonly object _synchronizationObject = new object();

        internal Guid GetGuid(DateTime now)
        {
            if (now < SequenceStartDate || now > SequenceEndDate)
            {
                return Guid.NewGuid(); // Outside the range, use regular Guid
            }

            var sequence = GetCurrentSequence(now);
            return GetGuid(sequence);
        }

        internal Guid GetGuid(long sequence)
        {
            lock (_synchronizationObject)
            {
                if (sequence <= _lastSequence)
                {
                    // Prevent double sequence on same server
                    sequence = _lastSequence + 1;
                }
                _lastSequence = sequence;
            }

            var sequenceBytes = GetSequenceBytes(sequence);
            var guidBytes = GetGuidBytes();
            var totalBytes = guidBytes.Concat(sequenceBytes).ToArray();
            var result = new Guid(totalBytes);
            return result;
        }

        private IEnumerable<byte> GetSequenceBytes(long sequence)
        {
            var sequenceBytes = BitConverter.GetBytes(sequence);
            var sequenceBytesLongEnough = sequenceBytes.Concat(new byte[NumberOfBytes]);
            var result = sequenceBytesLongEnough.Take(NumberOfBytes).Reverse();
            return result;
        }

        private static IEnumerable<byte> GetGuidBytes()
        {
            var result = Guid.NewGuid().ToByteArray().Take(10).ToArray();
            return result;
        }
    }
}
