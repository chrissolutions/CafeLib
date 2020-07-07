using CafeLib.Core.Eventing;

namespace CafeLib.Web.SignalR
{
    public class SignalRChangedMessage : EventMessage
    {
        public SignalRChannel Channel { get; }

        internal SignalRChangedMessage(SignalRChannel channel)
        {
            Channel = channel;
        }
    }
}
