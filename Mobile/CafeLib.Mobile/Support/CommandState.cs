using System.Collections.Generic;
using System.Windows.Input;
using CafeLib.Core.Extensions;
using CafeLib.Core.Support;

namespace CafeLib.Mobile.Support
{
    internal class CommandState : SingletonBase<CommandState>
    {
        private readonly Dictionary<int, ThreadSafeBool> _commandStateMap = new Dictionary<int, ThreadSafeBool>();

        public static bool IsSuppressed(ICommand command)
        {
            var key = command.GetHashCode();
            var state = Instance._commandStateMap.GetOrAdd(key, () => new ThreadSafeBool(false));
            return state.Get();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>false if safe to execute, true means cannot execute</returns>
        public static void Suppress(ICommand command)
        {
            var key = command.GetHashCode();
            var state = Instance._commandStateMap.GetOrAdd(key, new ThreadSafeBool());
            state.Set(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static void Release(ICommand command)
        {
            var key = command.GetHashCode();
            if (Instance._commandStateMap.TryGetValue(key, out var state))
            {
                state.Set(false);
            }
        }
    }
}
