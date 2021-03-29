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

        public Retry(int limit)
            : this(limit, DefaultInterval)
        {
        }

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
        public async Task Do(Action<int> action)
        {
            await Do(async x =>
            {
                action(x);
                return await Task.FromResult(0);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Do retries on function if necessary.
        /// </summary>
        /// <typeparam name="T">result type</typeparam>
        /// <param name="function">function</param>
        /// <returns>awaitable task</returns>
        public async Task<T> Do<T>(Func<int, T> function)
        {
            return await Do(async x =>
            {
                var result = function(x);
                return await Task.FromResult(result).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Do retries on function if necessary.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="function">retry function</param>
        /// <returns>the action return result</returns>
        public async Task<T> Do<T>(Func<int, Task<T>> function)
        {
            var exceptions = new List<Exception>();

            for (var retry = 0; retry < Limit; retry++)
            {
                try
                {
                    return await function(retry + 1).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    return await Task.FromResult(default(T)).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    await Task.Delay(Interval).ConfigureAwait(false);
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
        public static Task Run(Action<int> action)
        {
            return new Retry().Do(action);
        }

        /// <summary>
        /// Run retry action.
        /// </summary>
        /// <param name="limit">retry limit</param>
        /// <param name="action">retry action</param>
        /// <returns>asynchronous task</returns>
        public static Task Run(int limit, Action<int> action)
        {
            return new Retry(limit, DefaultInterval).Do(action);
        }

        /// <summary>
        /// Run retry action.
        /// </summary>
        /// <param name="limit">retry limit</param>
        /// <param name="interval">interval between retries</param>
        /// <param name="action">retry action</param>
        /// <returns>asynchronous task</returns>
        public static Task Run(int limit, int interval, Action<int> action)
        {
            return new Retry(limit, interval).Do(action);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="function">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(Func<int, T> function)
        {
            return new Retry().Do(function);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="limit">retry limit</param>
        /// <param name="function">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(int limit, Func<int, T> function)
        {
            return new Retry(limit, DefaultInterval).Do(function);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="limit">retry limit</param>
        /// <param name="interval">interval between retries</param>
        /// <param name="function">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(int limit, int interval, Func<int, T> function)
        {
            return new Retry(limit, interval).Do(function);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="function">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(Func<int, Task<T>> function)
        {
            return new Retry().Do(function);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="limit">retry limit</param>
        /// <param name="function">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(int limit, Func<int, Task<T>> function)
        {
            return new Retry(limit, DefaultInterval).Do(function);
        }

        /// <summary>
        /// Run retry function.
        /// </summary>
        /// <typeparam name="T">return type</typeparam>
        /// <param name="limit">retry limit</param>
        /// <param name="interval">interval between retries</param>
        /// <param name="function">retry function</param>
        /// <returns>asynchronous task</returns>
        public static Task<T> Run<T>(int limit, int interval, Func<int, Task<T>> function)
        {
            return new Retry(limit, interval).Do(function);
        }

        #endregion
    }
}
