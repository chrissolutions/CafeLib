using CafeLib.Core.Eventing;

namespace CafeLib.Web.SignalR
{
    public class SignalRMessage : EventMessage
    {
        public SignalRChannel Channel { get; }

        internal SignalRMessage(SignalRChannel channel)
        {
            Channel = channel;
        }
    }
}
