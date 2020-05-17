using System;
using CafeLib.Core.Eventing;
using CafeLib.Mobile.ViewModels;

namespace CafeLib.Mobile.Messages
{
    public class ViewModelCloseMessage : EventMessage
    {
        public object Parameter { get; }

        public Type TargetType { get; protected set; }

        public ViewModelCloseMessage(BaseViewModel sender)
            : base(sender)
        {
        }

        public ViewModelCloseMessage(BaseViewModel sender, object parameter)
            : base(sender)
        {
            Parameter = parameter;
        }
    }

    public class ViewModelCloseMessage<T> : ViewModelCloseMessage where T : BaseViewModel
    {
        public ViewModelCloseMessage(T sender)
            : base(sender)
        {
        }

        public ViewModelCloseMessage(T sender, object parameter) 
            : base(sender, parameter)
        {
        }
    }
}