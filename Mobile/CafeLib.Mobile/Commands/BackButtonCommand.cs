﻿using System;
using System.Diagnostics.CodeAnalysis;
using CafeLib.Mobile.ViewModels;

namespace CafeLib.Mobile.Commands
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class BackButtonCommand : XamCommand<NavigationSource, bool>
    {
        public BackButtonCommand(bool result = false)
            : base(_ => result)
        {
        }

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