using System;
using System.Threading;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Support
{
    /// <summary>
    /// Timekeeper timer.
    /// </summary>
    public sealed class Timekeeper : IDisposable
    {
        #region Private Variables

        private int _dueTime;
        private int _interval;
        private Timer _timer;
        private readonly Action _timekeeperEvent;
        private bool _disposed;

        public const int ImmediateStart = 0;

        #endregion

        #region Properties

        public bool Enabled { get; private set; }

        public int Period { get; private set; }

        public bool IsPeriodic { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Timekeeper constructor
        /// </summary>
        /// <param name="period">timekeeper period</param>
        /// <param name="timekeeperEvent">timekeeper event callback</param>
        public Timekeeper(int period, Action timekeeperEvent)
            : this(period, period, true, timekeeperEvent)
        {
        }

        /// <summary>
        /// Timekeeper constructor
        /// </summary>
        /// <param name="period">timekeeper period</param>
        /// <param name="initialStart">initial start</param>
        /// <param name="timekeeperEvent">timekeeper event callback</param>
        public Timekeeper(int period, int initialStart, Action timekeeperEvent)
            : this(period, initialStart, true, timekeeperEvent)
        {
        }

        /// <summary>
        /// Timekeeper constructor
        /// </summary>
        /// <param name="period">timekeeper period</param>
        /// <param name="periodic">periodic or one-time timekeeper</param>
        /// <param name="timekeeperEvent">timekeeper event callback</param>
        public Timekeeper(int period, bool periodic, Action timekeeperEvent)
            : this(period, period, periodic, timekeeperEvent)
        {
        }

        /// <summary>
        /// Timekeeper constructor
        /// </summary>
        /// <param name="period">timekeeper period</param>
        /// <param name="initialStart">initial start</param>
        /// <param name="periodic">periodic or one-time timekeeper</param>
        /// <param name="timekeeperEvent">timekeeper event callback</param>
        public Timekeeper(int period, int initialStart, bool periodic, Action timekeeperEvent)
        {
            Period = period;
            IsPeriodic = periodic;

            _interval = periodic ? period : Timeout.Infinite;
            _dueTime = initialStart;
            _timekeeperEvent = timekeeperEvent;
            _timer = new Timer(Callback, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Timekeeper finalizer.
        /// </summary>
        ~Timekeeper()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start timekeeper.
        /// </summary>
        public void Start()
        {
            StartTimer(_dueTime);
        }

        /// <summary>
        /// Start timekeeper.
        /// </summary>
        public void StartImmediate()
        {
            StartTimer(ImmediateStart);
        }

        /// <summary>
        /// Pause timekeeper.
        /// </summary>
        public void Stop()
        {
            Enabled = false;
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Reset timekeeper.
        /// </summary>
        public void Reset(bool immediateStart = false)
        {
            StartTimer(immediateStart ? 0 : _dueTime);
        }

        /// <summary>
        /// Change timekeeper settings.
        /// </summary>
        /// <param name="period">timekeeper period</param>
        /// <param name="periodic">periodic timekeeper</param>
        public void Change(int period, bool periodic = true)
        {
            _dueTime = period;
            Period = period;
            IsPeriodic = periodic;
            _interval = IsPeriodic ? period : Timeout.Infinite;
        }

        #endregion

        #region Timer Callback

        /// <summary>
        /// timekeeper event Callback 
        /// </summary>
        /// <param name="state">timer state</param>
        private void Callback(object state)
        {
            // Safely call timekeeper event.
            _timekeeperEvent?.Invoke();

            // Set timer period.
            if (_dueTime != Period)
            {
                Change(Period);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Start the timekeeper.
        /// </summary>
        /// <param name="initialStart">start timer indicator.</param>
        private void StartTimer(int initialStart)
        {
            _timer?.Change(initialStart, _interval);
            Enabled = true;
        }

        #endregion 

        #region IDisposible

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(!_disposed);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose concurrent queue.
        /// </summary>
        /// <param name="disposing">indicate whether the queue is disposing</param>
        private void Dispose(bool disposing)
        {
            if (!disposing) return;
            Stop();
            _timer.Dispose();
            _timer = null;
        }

        #endregion
    }
}
