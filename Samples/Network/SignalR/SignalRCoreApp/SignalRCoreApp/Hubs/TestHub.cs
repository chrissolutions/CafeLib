using System;
using System.Threading.Tasks;
using CafeLib.Web.SignalR.Hubs;

namespace SignalRCoreApp.Hubs
{
    public class TestHub : SignalRChannelHub
    {
        public Task NotifyMessage(string data)
        {
            return BroadcastAsync("notifyMessage", data);
        }

        public Task DoClientStuff(string name, string data)
        {
            const int intData = 5;
            return BroadcastAsync("doClientStuff", intData, data);
        }

        public Task DoGuidTest(string data)
        {
            return BroadcastAsync("doGuidTest", Guid.NewGuid(), data);
        }
    }
}
