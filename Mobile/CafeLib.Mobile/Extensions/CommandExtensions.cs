using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CafeLib.Mobile.Commands;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Extensions
{
    public static class CommandExtensions
    {
        /// <summary>
        /// Execute command asynchronously.
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>task</returns>
        public static Task ExecuteAsync(this ICommand command)
        {
            switch (command)
            {
                case XamAsyncCommand cmd0:
                    return cmd0.ExecuteAsync();

                case XamAsyncCommand<object> cmd1:
                    return cmd1.ExecuteAsync(null);

                default:
                    throw new InvalidOperationException(nameof(command));
            }
        }

        /// <summary>
        /// Execute command asynchronously.
        /// </summary>
        /// <typeparam name="T">parameter type</typeparam>
        /// <param name="command">command</param>
        /// <param name="parameter">command parameter</param>
        /// <returns>task</returns>
        public static Task ExecuteAsync<T>(this ICommand command, T parameter)
        {
            return ((XamAsyncCommand<T>)command).ExecuteAsync(parameter);
        }
    }
}
