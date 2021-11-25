using System.Diagnostics.CodeAnalysis;
using CafeLib.Mobile.Commands;
using CafeLib.Mobile.Views;

namespace CafeLib.Mobile.ViewModels
{
    public abstract class BaseFlyoutViewModel<TObject> : BaseViewModel<TObject> where TObject : class
    {
        protected BaseFlyoutViewModel()
        {
            PresentedCommand = new XamCommand<bool>(_ => { });
        }

        private bool _isPresented;
        public bool IsPresented
        {
            get => _isPresented;
            set
            {
                if (!SetValue(ref _isPresented, value)) return;
                ResolvePage<CafeFlyoutPage>().IsPresented = value;
                _presentedCommand.Execute(_isPresented);
            }
        }

        /// <summary>
        /// Appearing command.
        /// </summary>
        private IXamCommand<bool> _presentedCommand;
        public IXamCommand<bool> PresentedCommand
        {
            set => SetValue(ref _presentedCommand, value);
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public abstract class BaseFlyoutViewModel : BaseFlyoutViewModel<object>
    {
    }
}