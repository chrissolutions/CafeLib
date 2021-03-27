using CafeLib.Mobile.Commands;
using CafeLib.Mobile.Views;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.ViewModels
{
    public abstract class BaseFlyoutViewModel<TObject> : BaseViewModel<TObject> where TObject : class
    {
        protected BaseFlyoutViewModel()
        {
            PresentedCommand = new XamCommand<bool>(x => { });
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

    public abstract class BaseFlyoutViewModel : BaseFlyoutViewModel<object>
    {
    }
}