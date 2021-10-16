using CafeLib.AspNet.WebSockets;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace WebSocketChatServer
{
    public class ChatMessageHandler : WebSocketHandler
    {
        public ChatMessageHandler(IServiceProvider provider)
            : base(provider)
        {
        }

        public override async Task OnConnect(Guid connectionId)
        {
            await BroadcastMessageAsync($"{connectionId} is now connected");
        }

        public override Task OnDisconnect(Guid connectionId)
        {
            return Task.CompletedTask;
        }

        public override async Task ReceiveMessageAsync(Guid connectionId, string message)
        {
            await BroadcastMessageAsync($"{connectionId} said: {message}");
        }
    }
}