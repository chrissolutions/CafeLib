using System;
using System.Threading;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Runnable
{
    public class RecurrentTask : RecurrentTaskUtc
    {
        /// <summary>
        /// RecurrentTask constructor.
        /// </summary>
        /// <param name="task">runnable task</param>
        /// <param name="interval">interval in milliseconds</param>
        /// <param name="startTime">start time</param>
        /// <param name="frequency">frequency in milliseconds (default is 1 second)</param>
        public RecurrentTask(Func<CancellationToken, Task> task, int interval, DateTime startTime = default, int frequency = 1000)
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
        public RecurrentTask(Func<CancellationToken, Task> task, TimeSpan interval, DateTime startTime, TimeSpan frequency)
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
        public RecurrentTask(Func<CancellationToken, Task> task, TimeSpan interval, DateTime startTime = default, int frequency = 1000)
            : base(task, interval, startTime.ToUniversalTime(), frequency)
        {
        }
    }
}