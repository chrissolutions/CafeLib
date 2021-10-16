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
            return command switch
            {
                XamAsyncCommand cmd0 => cmd0.ExecuteAsync(),
                XamAsyncCommand<object> cmd1 => cmd1.ExecuteAsync(null),
                _ => throw new InvalidOperationException(nameof(command))
            };
        }
    }
}
