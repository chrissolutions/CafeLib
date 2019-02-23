using System;
using System.Threading;

namespace CafeLib.Core.Diagnostics
{
    /// <summary>
    /// Watchdog timer.
    /// </summary>
    public sealed class Watchdog : IDisposable
    {
        #region Private Variables

        private int _dueTime;
        private int _interval;
        private Timer _timer;
        private readonly Action _watchdogEvent;
        private bool _disposed;

        private const int ImmediateStart = 0;

        #endregion

        #region Properties

        public bool Enabled { get; private set; }

        public int Period { get; private set; }

        public bool IsPeriodic { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Watchdog constructor
        /// </summary>
        /// <param name="period">watchdog period</param>
        /// <param name="watchdogEvent">watchdog event callback</param>
        public Watchdog(int period, Action watchdogEvent)
            : this(period, ImmediateStart, true, watchdogEvent)
        {
        }

        /// <summary>
        /// Watchdog constructor
        /// </summary>
        /// <param name="period">watchdog period</param>
        /// <param name="initialStart">initial start</param>
        /// <param name="watchdogEvent">watchdog event callback</param>
        public Watchdog(int period, int initialStart, Action watchdogEvent)
            : this(period, initialStart, true, watchdogEvent)
        {
        }

        /// <summary>
        /// Watchdog constructor
        /// </summary>
        /// <param name="period">watchdog period</param>
        /// <param name="periodic">periodic or one-time watchdog</param>
        /// <param name="watchdogEvent">watchdog event callback</param>
        public Watchdog(int period, bool periodic, Action watchdogEvent)
            : this(period, ImmediateStart, periodic, watchdogEvent)
        {
        }

        /// <summary>
        /// Watchdog constructor
        /// </summary>
        /// <param name="period">watchdog period</param>
        /// <param name="initialStart">initial start</param>
        /// <param name="periodic">periodic or one-time watchdog</param>
        /// <param name="watchdogEvent">watchdog event callback</param>
        public Watchdog(int period, int initialStart, bool periodic, Action watchdogEvent)
        {
            Period = period;
            IsPeriodic = periodic;

            _interval = periodic ? period : Timeout.Infinite;
            _dueTime = initialStart;
            _watchdogEvent = watchdogEvent;
            _timer = new Timer(Callback, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Watchdog finalizer.
        /// </summary>
        ~Watchdog()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start watchdog.
        /// </summary>
        public void Start()
        {
            StartTimer(true);
        }

        /// <summary>
        /// Pause watchdog.
        /// </summary>
        public void Stop()
        {
            Enabled = false;
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Reset watchdog.
        /// </summary>
        /// <param name="immediateStart">immediateStart flag</param>
        public void Reset(bool immediateStart = false)
        {
            StartTimer(immediateStart);
        }

        /// <summary>
        /// Change watchdog settings.
        /// </summary>
        /// <param name="period">watchdog period</param>
        /// <param name="periodic">periodic watchdog</param>
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
        /// Watchdog event Callback 
        /// </summary>
        /// <param name="state">timer state</param>
        private void Callback(object state)
        {
            // Safely call watchdog event.
            _watchdogEvent?.Invoke();

            // Set timer period.
            if (_dueTime != Period)
            {
                Change(Period);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Start the watchdog timer.
        /// </summary>
        /// <param name="immediateStart">start timer indicator.</param>
        private void StartTimer(bool immediateStart)
        {
            _timer?.Change(immediateStart ? ImmediateStart : _dueTime, _interval);
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
