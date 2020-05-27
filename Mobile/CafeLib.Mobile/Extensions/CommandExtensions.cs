using System;
using System.Threading.Tasks;
using System.Windows.Input;
using CafeLib.Mobile.Commands;
using CafeLib.Mobile.Support;

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

        public static bool IsSuppressed(this ICommand command)
        {
            return CommandState.IsSuppressed(command);
        }

        public static void Suppress(this ICommand command)
        {
            CommandState.Suppress(command);
        }

        public static void Release(this ICommand command)
        {
            CommandState.Release(command);
        }
    }
}
