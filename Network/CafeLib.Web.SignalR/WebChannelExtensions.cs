using System;
using System.Threading;
using System.Threading.Tasks;
using CafeLib.Core.Extensions;
using Microsoft.AspNetCore.SignalR.Client;

namespace CafeLib.Web.SignalR
{
    public static class WebChannelExtensions
    {
        public static IDisposable On(this WebChannel channel, string methodName, Action handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1>(this WebChannel channel, string methodName, Action<T1> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2>(this WebChannel channel, string methodName, Action<T1, T2> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2, T3>(this WebChannel channel, string methodName, Action<T1, T2, T3> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2, T3, T4>(this WebChannel channel, string methodName, Action<T1, T2, T3, T4> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2, T3, T4, T5>(this WebChannel channel, string methodName, Action<T1, T2, T3, T4, T5> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            connection.Value.On(methodName, handler);
            return null;
        }

        public static IDisposable On<T1, T2, T3, T4, T5, T6>(this WebChannel channel, string methodName, Action<T1, T2, T3, T4, T5, T6> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2, T3, T4, T5, T6, T7>(this WebChannel channel, string methodName, Action<T1, T2, T3, T4, T5, T6, T7> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On<T1, T2, T3, T4, T5, T6, T7, T8>(this WebChannel channel, string methodName, Action<T1, T2, T3, T4, T5, T6, T7, T8> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, handler);
        }

        public static IDisposable On(this WebChannel channel, string methodName, Type[] parameterTypes, Func<object[], Task> handler)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            return connection.Value.On(methodName, parameterTypes, handler);
        }

        public static async Task InvokeAsync(this WebChannel channel, string methodName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            await connection.Value.InvokeAsync(methodName, cancellationToken);
        }

        public static async Task InvokeAsync(this WebChannel channel, string methodName, params object[] args)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            await connection.Value.InvokeAsync(methodName, args);
        }

        public static async Task InvokeAsync(this WebChannel channel, CancellationToken cancellationToken, string methodName, params object[] args)
        {
            var connection = new NonNullable<HubConnection>(channel.Connection);
            await connection.Value.InvokeAsync(methodName, args, cancellationToken);
        }
    }
}
