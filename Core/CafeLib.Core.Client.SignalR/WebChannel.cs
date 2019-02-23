using System;
using System.Threading.Tasks;
using CafeLib.Core.Binding;
using CafeLib.Core.Diagnostics;
using CafeLib.Core.Logging;
using CafeLib.Core.Runnable;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Protocol;

namespace CafeLib.Core.Client.SignalR
{
    public class WebChannel : Runner, ILogEventMessenger
    {
        #region Private Variables

        private const int DefaultDelay = 10;  // 10 millisec default delay.
        private const int CheckConnectionInterval = 5000; // Check connection every 5 sec.
        private readonly WebChannelLogFactory _logFactory;
        private Action<LogEventMessage> _eventListener;

        #endregion

        #region Constants

        private const int ConnectionStateId = 26;

        #endregion

        #region Automatic Properties
        protected MethodBridge Bridge { get; set; }

        internal HubConnection Connection { get; private set; }

        public Uri Url { get; }
        public WebChannelConnectionState ConnectionState { get; private set; }
        public bool IsConnected => ConnectionState == WebChannelConnectionState.Connected;
        public int ConnectionAttempts { get; private set; }

        public Action<LogEventMessage> ChannelEvent
        {
            get => _eventListener;
            set { _eventListener = value ?? delegate { };}
        }

        #endregion

        #region Constructors

        /// <summary>
        /// WebChannel constructor
        /// </summary>
        /// <param name="url">Channel url</param>
        /// <param name="bridge">Method bridge</param>
        /// <param name="eventListener">event listener</param>
        public WebChannel(string url, MethodBridge bridge, Action<LogEventMessage> eventListener = null)
            : base(DefaultDelay)
        {
            _logFactory = new WebChannelLogFactory(url, this);
            Url = new Uri(url);
            Bridge = bridge;
            ChannelEvent = eventListener;
            ConnectionState = WebChannelConnectionState.Off;
            RunnerEvent = x =>
            {
                if (!(x is RunnerEventMessage message) || message.ErrorLevel == ErrorLevel.Ignore) return;
                LogMessage(new LogEventMessage(Url.ToString(), message.ErrorLevel, message.Message));
            };
        }

        #endregion

        #region Methods

        public override async Task Start()
        {
            await ConnectChannel();
            await base.Start();
        }

        protected override async Task Run()
        {
            try
            {
                await ConnectChannel();
            }
            catch (Exception ex)
            {
                LogMessage(new LogEventMessage(Url.ToString(), ex));
            }
        }

        /// <summary>
        /// Forward log message to log event listener.
        /// </summary>
        /// <typeparam name="T">event message type</typeparam>
        /// <param name="message">event message</param>
        public void LogMessage<T>(T message) where T : LogEventMessage
        {
            SetState(message as WebChannelLogEventMessage);
            ChannelEvent.Invoke(message);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Connect the web channel.
        /// </summary>
        /// <returns>task</returns>
        private async Task ConnectChannel()
        {
            if (IsConnected) return;

            Connection = new HubConnection(new WebChannelConnectionFactory(Url, _logFactory), new JsonHubProtocol(), _logFactory);

            Connection.On("client", (string methodName, object[] args) =>
            {
                Bridge?.Invoke(methodName, args);
            });

            await Connection.StartAsync();
            ConnectionState = WebChannelConnectionState.Connected;
            Delay = CheckConnectionInterval;
        }

        /// <summary>
        /// Set the connection state.
        /// </summary>
        /// <param name="message">HttpConnection message to parse for connection state.</param>
        private void SetState(WebChannelLogEventMessage message)
        {
            if (message?.EventInfo.Id != ConnectionStateId || message.EventInfo.Name != "ConnectionStateChanged") return;
            ConnectionState = (WebChannelConnectionState)Enum.Parse(typeof(WebChannelConnectionState), message.MessageInfo["newState"].ToString());

            switch (ConnectionState)
            {
                case WebChannelConnectionState.Connected:
                    ConnectionAttempts = 0;
                    break;

                case WebChannelConnectionState.Disconnected:
                    break;

                case WebChannelConnectionState.Connecting:
                case WebChannelConnectionState.Reconnecting:
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
