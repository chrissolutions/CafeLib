using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Support
{
    public class Retry
    {
        #region Constants

        private const int DefaultLimit = 3;
        private const int DefaultInterval = 50; // milliseconds

        #endregion

        #region Automatic Properties.

        /// <summary>
        /// Retry attempts limit.
        /// </summary>
        public int Limit { get; }

        /// <summary>
        /// Retry interval between attempts.
        /// </summary>
        public int Interval { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Retry constructor.
        /// </summary>
        /// <param name="limit">retry limit</param>
        /// <param name="interval">interval between retries</param>
        public Retry(int limit = DefaultLimit, int interval = DefaultInterval)
        {
            Limit = limit > 0 ? limit : 1;
            Interval = interval > 0 ? interval : DefaultInterval;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Do retries on action if necessary.
        /// </summary>
        /// <param name="action">action</param>
        /// <returns>awaitable task</returns>
        public async Task Do(Action action)
        {
            await Do(async () =>
            {
                action();
                return await Task.FromResult(0);
            });
        }

        /// <summary>
        /// Do retries on function if necessary.
        /// </summary>
        /// <typeparam name="T">result type</typeparam>
        /// <param name="function">function</param>
        /// <returns>awaitable task</returns>
        public async Task<T> Do<T>(Func<T> function)
        {
            return await Do(async () =>
            {
                var result = function();
                return await Task.FromResult(result);
            });
        }

        /// <summary>
        /// Do retries on function if necessary.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="function">retry function</param>
        /// <returns>the action return result</returns>
        public async Task<T> Do<T>(Func<Task<T>> function)
        {
            var exceptions = new List<Exception>();

            for (var retry = 0; retry < Limit; retry++)
            {
                try
                {
                    return await function();
                }
                catch (TaskCanceledException)
                {
                    return await Task.FromResult(default(T));
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    await Task.Delay(Interval);
                }
            }

            throw new AggregateException(exceptions);
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Run retry action.
        /// </summary>
        /// <param name="action">retry action</param>
        /// <returns>asynchronous task</returns>
        public static Task Run(Action action)
        {
            return new Retry().Do(action);
        }

        /// <summary>
        /// Run retry action.
        /// </summary>
        /// <param name="limit">retry limit</param>
        /// <param name="interval">interval between retries</param>
        /// <param name="action">retry action</param>
        /// <returns>asynchronous task</returns>
        public static Task Run(int limit, int interval, Action action)
        {
            return new Retry(limit, interval).Do(action);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="function">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(Func<T> function)
        {
            return new Retry().Do(function);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="limit">retry limit</param>
        /// <param name="interval">interval between retries</param>
        /// <param name="function">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(int limit, int interval, Func<T> function)
        {
            return new Retry(limit, interval).Do(function);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="function">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(Func<Task<T>> function)
        {
            return new Retry().Do(function);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="limit">retry limit</param>
        /// <param name="interval">interval between retries</param>
        /// <param name="function">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(int limit, int interval, Func<Task<T>> function)
        {
            return new Retry(limit, interval).Do(function);
        }

        #endregion
    }
}
