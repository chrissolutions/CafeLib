using System;
using CafeLib.Mobile.ViewModels;

namespace CafeLib.Mobile.Commands
{
    public class BackButtonCommand : XamCommand<NavigationSource, bool>
    {
        public BackButtonCommand(Action<NavigationSource> command)
            : base(p => { command(p); return true; })
        {
        }

        public BackButtonCommand(Action<NavigationSource> command, Func<NavigationSource, bool> canExecute)
            : base(p => { command(p); return true; }, canExecute)
        {
        }
    }
}
