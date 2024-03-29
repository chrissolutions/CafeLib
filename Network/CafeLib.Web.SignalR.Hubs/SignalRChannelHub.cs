﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace CafeLib.Web.SignalR.Hubs
{
    public class SignalRChannelHub : Hub
    {
        public Task BroadcastAsync(string methodName, params object[] args)
        {
            var arguments = args.Any() ? args.ToArray() : Array.Empty<object>();
            return Clients.All.SendAsync("client", methodName, arguments);
        }
    }
}
