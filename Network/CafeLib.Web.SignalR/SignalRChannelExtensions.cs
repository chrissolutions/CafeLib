using System;
using System.Threading.Tasks;
using CafeLib.Core.Support;
using Microsoft.AspNetCore.SignalR.Client;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Web.SignalR
{
    public static class SignalRChannelExtensions
    {
        public static IDisposable On(this SignalRChannel channel, string methodName, Action handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1>(this SignalRChannel channel, string methodName, Action<T1> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2>(this SignalRChannel channel, string methodName, Action<T1, T2> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2, T3>(this SignalRChannel channel, string methodName, Action<T1, T2, T3> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2, T3, T4>(this SignalRChannel channel, string methodName, Action<T1, T2, T3, T4> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2, T3, T4, T5>(this SignalRChannel channel, string methodName, Action<T1, T2, T3, T4, T5> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            connection.Value.On(methodName, handler);
            return null;
        }

        public static IDisposable On<T1, T2, T3, T4, T5, T6>(this SignalRChannel channel, string methodName, Action<T1, T2, T3, T4, T5, T6> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2, T3, T4, T5, T6, T7>(this SignalRChannel channel, string methodName, Action<T1, T2, T3, T4, T5, T6, T7> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2, T3, T4, T5, T6, T7, T8>(this SignalRChannel channel, string methodName, Action<T1, T2, T3, T4, T5, T6, T7, T8> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On(this SignalRChannel channel, string methodName, Type[] parameterTypes, Func<object[], Task> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, parameterTypes, handler);
        }
    }
}
