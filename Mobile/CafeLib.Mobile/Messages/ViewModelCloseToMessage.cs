using CafeLib.Mobile.ViewModels;

namespace CafeLib.Mobile.Messages
{
    public class ViewModelCloseToMessage<T> : ViewModelCloseMessage where T : BaseViewModel
    {
        public ViewModelCloseToMessage(BaseViewModel sender)
            : base(sender)
        {
            TargetType = typeof(T);
        }
    }
}