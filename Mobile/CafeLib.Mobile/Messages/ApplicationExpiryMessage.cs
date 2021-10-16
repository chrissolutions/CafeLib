using CafeLib.Core.Eventing;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.Messages
{
    public class ApplicationExpiryMessage : EventMessage
    {
        public ApplicationExpiryMessage(object sender = null)
            : base(sender)
        {
        }
    }
}
