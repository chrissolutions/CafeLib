using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Core.Support
{
    public static class AsyncTask
    {
        /// <summary>
        /// Task.WhenAll wrapper that returns an aggregate of all of the exceptions that occurs in a task collection.
        /// </summary>
        /// <param name="tasks">collection of tasks</param>
        /// <returns>enumerable of type T</returns>
        public static async Task WhenAll(params Task[] tasks)
        {
            var allTasks = Task.WhenAll(tasks);

            try
            {
                await allTasks;
                return;
            }
            catch (Exception)
            {
                // ignore
            }

            throw allTasks.Exception ?? throw new Exception();
        }

        /// <summary>
        /// Task.WhenAll wrapper that returns an aggregate of all of the exceptions that occurs in a task collection.
        /// </summary>
        /// <typeparam name="T">collection type</typeparam>
        /// <param name="tasks">collection of tasks</param>
        /// <returns>enumerable of type T</returns>
        public static async Task<IEnumerable<T>> WhenAll<T>(params Task<T>[] tasks)
        {
            var allTasks = Task.WhenAll(tasks);

            try
            {
                return await allTasks;
            }
            catch (Exception)
            {
                // ignore
            }

            throw allTasks.Exception ?? throw new Exception();
        }
    }
}