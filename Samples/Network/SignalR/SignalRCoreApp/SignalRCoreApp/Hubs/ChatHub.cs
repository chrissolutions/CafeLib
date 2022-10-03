using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRCoreApp.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            await Clients.All.SendAsync("broadcastMessage", name, message);
        }

        //public async Task Send(params object[] data)
        //{
        //    // Call the broadcastMessage method to update clients.
        //    await Clients.All.SendAsync("broadcastMessage", data);
        //}
    }
}