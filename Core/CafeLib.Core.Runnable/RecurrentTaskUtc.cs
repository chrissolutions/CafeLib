using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Extensions;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Runnable
{
    public class RecurrentTaskUtc : RunnerBase
    {
        private readonly Func<CancellationToken, Task> _task;
        private readonly TimeSpan _interval;
        private DateTime _triggerTimeUtc;

        /// <summary>
        /// RecurrentTask constructor.
        /// </summary>
        /// <param name="task">runnable task</param>
        /// <param name="interval">interval in milliseconds</param>
        /// <param name="startTimeUtc">start time</param>
        /// <param name="frequency">frequency in milliseconds (default is 1 second)</param>
        public RecurrentTaskUtc(Func<CancellationToken, Task> task, int interval, DateTime startTimeUtc = default, int frequency = 1000)
            : this(task, TimeSpan.FromMilliseconds(interval), startTimeUtc, frequency)
        {
        }

        /// <summary>
        /// RecurrentTask constructor.
        /// </summary>
        /// <param name="task">runnable task</param>
        /// <param name="interval">interval</param>
        /// <param name="startTimeUtc">start time</param>
        /// <param name="frequency"></param>
        public RecurrentTaskUtc(Func<CancellationToken, Task> task, TimeSpan interval, DateTime startTimeUtc, TimeSpan frequency)
            : this(task, interval, startTimeUtc, (int)frequency.TotalMilliseconds)
        {
        }

        /// <summary>
        /// RecurrentTask constructor.
        /// </summary>
        /// <param name="task">runnable task</param>
        /// <param name="interval">interval</param>
        /// <param name="startTimeUtc">start time</param>
        /// <param name="frequency"></param>
        public RecurrentTaskUtc(Func<CancellationToken, Task> task, TimeSpan interval, DateTime startTimeUtc = default, int frequency = 1000)
            : base(frequency)
        {
            _task = task ?? (_ => Task.CompletedTask);
            _interval = interval;
            _triggerTimeUtc = GetTriggerTime(startTimeUtc);
        }

        /// <summary>
        /// Run the task.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>asynchronous task</returns>
        protected override async Task Run(CancellationToken token)
        {
            if (DateTime.Now >= _triggerTimeUtc)
            {
                await _task(token).ConfigureAwait(false);
                _triggerTimeUtc = GetTriggerTime(_triggerTimeUtc);
            }
        }

        /// <summary>
        /// Calculate new start time.
        /// </summary>
        /// <param name="startTimeUtc"></param>
        /// <returns></returns>
        private DateTime GetTriggerTime(DateTime startTimeUtc)
        {
            return startTimeUtc == default ? DateTime.UtcNow : startTimeUtc.NextTimeUtc(_interval);
        }
    }
}