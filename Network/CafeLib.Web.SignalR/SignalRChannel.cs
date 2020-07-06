using System;
using System.Threading.Tasks;
using CafeLib.Core.Logging;
using CafeLib.Core.MethodBinding;
using CafeLib.Core.Runnable;
using CafeLib.Core.Support;
using CafeLib.Web.SignalR.ConnectionFactory;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Logging;

namespace CafeLib.Web.SignalR
{
    public abstract class SignalRChannel : RunnerBase, IServiceProvider
    {
        #region Private Variables

        private const int DefaultDelay = 10;  // 10 millisecond default delay.
        private const int CheckConnectionInterval = 5000; // Check connection every 5 sec.

        #endregion

        #region Constants

        private const int ConnectionStateId = 26;

        #endregion

        #region Automatic Properties
        protected MethodBridge Bridge { get; set; }

        internal HubConnection Connection { get; private set; }

        protected ILogger Logger { get; }

        public Uri Url { get; }

        public SignalRChannelState ConnectionState { get; private set; }

        public int ConnectionAttempts { get; private set; }

        public bool IsConnected => ConnectionState == SignalRChannelState.Connected;

        #endregion

        #region Constructors

        /// <summary>
        /// SignalRChannel constructor
        /// </summary>
        /// <param name="url">Channel url</param>
        /// <param name="bridge">Method bridge</param>
        /// <param name="logger">event listener</param>
        protected SignalRChannel(string url, MethodBridge bridge, ILogger logger = null)
            : base(DefaultDelay)
        {
            Url = new Uri(url);
            Bridge = bridge;
            Logger = logger ?? new LogEventWriter<SignalRChannel>(x => { });
            ConnectionState = SignalRChannelState.Off;
            RunnerEvent = x =>
            {
                if (!(x is RunnerEventMessage message) || message.ErrorLevel == ErrorLevel.Ignore) return;
                LogEventListener(new LogEventMessage(Url.ToString(), message.ErrorLevel, message.Message));
            };
        }

        #endregion

        #region Methods

        public override async Task Start()
        {
            await OpenChannel();
            await base.Start();
        }

        public override async Task Stop()
        {
            await base.Stop();
            await CloseChannel();
        }

        protected override async Task Run()
        {
            try
            {
                await OpenChannel();
            }
            catch (Exception ex)
            {
                LogEventListener(new LogEventMessage(Url.ToString(), ex));
            }
        }

        public object GetService(Type serviceType)
        {
            return serviceType == GetType() ? this : throw new NotSupportedException(nameof(serviceType));
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Forward log message to log event listener.
        /// </summary>
        /// <param name="message">event message</param>
        private void LogEventListener(LogEventMessage message)
        {
            SetState(message);
            Logger.LogMessage(message.ErrorLevel, message.EventInfo, message.Message, message.Exception);
        }

        /// <summary>
        /// Connect the web channel.
        /// </summary>
        /// <returns>task</returns>
        private async Task OpenChannel()
        {
            if (IsConnected) return;

            var connectionFactory = this is SignalRTextChannel
                ? new SignalRTextConnectionFactory(LogEventListener)
                : new SignalRConnectionFactory(LogEventListener);

            var protocol = new JsonHubProtocol();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(new LoggerProvider(LogEventListener)));

            Connection = new HubConnection(connectionFactory, protocol, new UriEndPoint(Url), this, loggerFactory);

            Connection.On("client", (string methodName, object[] args) =>
            {
                Bridge?.Invoke(methodName, args);
            });

            await Connection.StartAsync();
            ConnectionState = SignalRChannelState.Connected;
            Delay = CheckConnectionInterval;
        }

        private async Task CloseChannel()
        {
            try
            {
                await Connection.StopAsync();
                await Connection.DisposeAsync();
            }
            catch
            {
                // ignored
            }
            finally
            {
                Connection = null;
            }
        }

        /// <summary>
        /// Set the connection state.
        /// </summary>
        /// <param name="message">HttpConnection message to parse for connection state.</param>
        private void SetState(LogEventMessage message)
        {
            if (message?.EventInfo.Id != ConnectionStateId || message.EventInfo.Name != "ConnectionStateChanged") return;
            ConnectionState = (SignalRChannelState)Enum.Parse(typeof(SignalRChannelState), message.MessageInfo["newState"].ToString());

            switch (ConnectionState)
            {
                case SignalRChannelState.Connected:
                    ConnectionAttempts = 0;
                    break;

                case SignalRChannelState.Disconnected:
                    break;

                case SignalRChannelState.Connecting:
                case SignalRChannelState.Reconnecting:
                    try
                    {
                        ++ConnectionAttempts;
                    }
                    catch
                    {
                        ConnectionAttempts = 0;
                    }
                    break;
            }
        }

        #endregion
    }
}
