using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CafeLib.Core.Support
{
    public static class Retry
    {
        #region Constants

        private const int DefaultLimit = 3;
        private const int DefaultInterval = 50; // milliseconds

        #endregion

        #region Static Methods

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <param name="operation">retry action</param>
        /// <returns>asynchronous task</returns>
        public static Task Run(Action<int> operation) 
            => Run(DefaultLimit, DefaultInterval, operation);

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <param name="limit">retry limit</param>
        /// <param name="operation">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task Run(int limit, Action<int> operation) 
            => Run(limit, DefaultInterval, operation);

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <param name="limit">retry limit</param>
        /// <param name="interval">interval between retries</param>
        /// <param name="operation">retry function</param>
        /// <returns>asynchronous task</returns>
        public static async Task Run(int limit, int interval, Action<int> operation)
        {
            await Run(limit, interval, async x =>
            {
                operation(x);
                await Task.CompletedTask;
            });
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="operation">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(Func<int, T> operation)
            => Run(DefaultLimit, DefaultInterval, operation);

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="limit">retry limit</param>
        /// <param name="operation">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(int limit, Func<int, T> operation)
            => Run(limit, DefaultInterval, operation);

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="limit">retry limit</param>
        /// <param name="interval">interval between retries</param>
        /// <param name="operation">retry function</param>
        /// <returns>asynchronous task</returns>
        public static async Task<T> Run<T>(int limit, int interval, Func<int, T> operation)
        {
            return await Run(limit, interval, x => Task.FromResult(operation(x)));
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="interval"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static async Task Run(int limit, int interval, Func<int, Task> operation)
        {
            var exceptions = new List<Exception>();

            for (var retry = 0; retry < limit; retry++)
            {
                try
                {
                    await operation(retry + 1);
                    return;
                }
                catch (TaskCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    await Task.Delay(interval);
                }
            }

            throw new AggregateException(exceptions);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="limit"></param>
        /// <param name="interval"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static async Task<T> Run<T>(int limit, int interval, Func<int, Task<T>> operation)
        {
            var exceptions = new List<Exception>();

            for (var retry = 0; retry < limit; retry++)
            {
                try
                {
                    return await operation(retry + 1);
                }
                catch (TaskCanceledException)
                {
                    return default;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    await Task.Delay(interval);
                }
            }

            throw new AggregateException(exceptions);
        }
        #endregion
    }
}
