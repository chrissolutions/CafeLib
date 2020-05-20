using System;
using CafeLib.Mobile.ViewModels;

namespace CafeLib.Mobile.Commands
{
    public class BackButtonCommand : XamCommand<NavigationSource, bool>
    {
        public BackButtonCommand(Action<NavigationSource> command, bool result = true)
            : base(p => { command(p); return result; })
        {
        }

        public BackButtonCommand(Action<NavigationSource> command, bool result, Func<NavigationSource, bool> canExecute)
            : base(p => { command(p); return result; }, canExecute)
        {
        }
    }
}