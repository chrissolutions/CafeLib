using CafeLib.Core.Eventing;

namespace CafeLib.Web.SignalR
{
    public class SignalRChangedMessage : EventMessage
    {
        public SignalRChannelState ConnectionState { get; }

        public int ConnectionAttempts { get; }

        internal SignalRChangedMessage(SignalRChannel channel)
        {
            ConnectionState = channel.ConnectionState;
            ConnectionAttempts = channel.ConnectionAttempts;
        }
    }
}
