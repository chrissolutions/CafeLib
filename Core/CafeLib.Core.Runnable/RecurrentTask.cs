using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Extensions;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Runnable
{
    public class RecurrentTask : RunnerBase
    {
        private readonly AsyncLocal<Func<Task>> _task;
        private readonly AsyncLocal<TimeSpan> _interval;
        private readonly AsyncLocal<DateTime> _triggerTime;

        private Func<Task> Task => _task.Value;

        private TimeSpan Interval => _interval.Value;

        private DateTime TriggerTime
        {
            get => _triggerTime.Value;
            set => _triggerTime.Value = value;
        }

        /// <summary>
        /// RecurrentTask constructor.
        /// </summary>
        /// <param name="task">runnable task</param>
        /// <param name="interval">interval in milliseconds</param>
        /// <param name="startTime">start time</param>
        /// <param name="frequency">frequency in milliseconds (default is 1 second)</param>
        public RecurrentTask(Func<Task> task, int interval, DateTime startTime = default, int frequency = 1000)
                : this(task, TimeSpan.FromMilliseconds(interval), startTime, frequency)
        {
        }

        /// <summary>
        /// RecurrentTask constructor.
        /// </summary>
        /// <param name="task">runnable task</param>
        /// <param name="interval">interval</param>
        /// <param name="startTime">start time</param>
        /// <param name="frequency"></param>
        public RecurrentTask(Func<Task> task, TimeSpan interval, DateTime startTime, TimeSpan frequency)
            : this(task, interval, startTime, (int)frequency.TotalMilliseconds)
        {
        }

        /// <summary>
        /// RecurrentTask constructor.
        /// </summary>
        /// <param name="task">runnable task</param>
        /// <param name="interval">interval</param>
        /// <param name="startTime">start time</param>
        /// <param name="frequency"></param>
        public RecurrentTask(Func<Task> task, TimeSpan interval, DateTime startTime = default, int frequency = 1000)
            : base(frequency)
        {
            _task = new AsyncLocal<Func<Task>> { Value = task ?? (() => System.Threading.Tasks.Task.CompletedTask) };
            _interval = new AsyncLocal<TimeSpan> { Value = interval };
            _triggerTime = new AsyncLocal<DateTime> { Value = GetTriggerTime(startTime) };
        }

        /// <summary>
        /// Run the task.
        /// </summary>
        /// <returns>asynchronous task</returns>
        protected override async Task Run()
        {
            if (DateTime.Now >= TriggerTime)
            {
                await Task();
                TriggerTime = GetTriggerTime(TriggerTime);
            }
        }

        /// <summary>
        /// Calculate new start time.
        /// </summary>
        /// <param name="startTime"></param>
        /// <returns></returns>
        private DateTime GetTriggerTime(DateTime startTime)
        {
            return startTime == default ? DateTime.Now : startTime.NextTime(Interval);
        }
    }
}
