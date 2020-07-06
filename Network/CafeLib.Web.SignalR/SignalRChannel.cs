using System;
using System.Security.Cryptography.X509Certificates;
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
    public class SignalRChannel : RunnerBase, IServiceProvider
    {
        #region Private Variables

        private const int DefaultDelay = 10;  // 10 millisecond default delay.
        private const int CheckConnectionInterval = 5000; // Check connection every 5 sec.

        #endregion

        #region Automatic Properties
        protected MethodBridge Bridge { get; set; }

        public HubConnection Connection { get; private set; }
        public HubConnectionBuilder Connection2 { get; private set; }

        protected ILogger Logger { get; }

        public Uri Url { get; }

        public SignalRChannelState ConnectionState { get; private set; }

        public int ConnectionAttempts { get; private set; }

        public bool IsConnected => ConnectionState == SignalRChannelState.Connected;
//        public bool IsConnected => Connection.State == HubConnectionState.Connected;

        public event Action<SignalRMessage> ChannelEvent;

        #endregion

        #region Constructors

        /// <summary>
        /// SignalRChannel constructor
        /// </summary>
        /// <param name="url">Channel url</param>
        /// <param name="bridge">Method bridge</param>
        /// <param name="logger">event listener</param>
        public SignalRChannel(string url, MethodBridge bridge, ILogger logger = null)
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
            SetState();
            ChannelEvent.Invoke(new SignalRMessage(this));
            Logger.LogMessage(message.ErrorLevel, message.EventInfo, message.Message, message.Exception);
        }

        /// <summary>
        /// Connect the web channel.
        /// </summary>
        /// <returns>task</returns>
        private async Task OpenChannel()
        {
            if (IsConnected) return;

            //Connection = new HubConnectionBuilder()
            //    .WithUrl(Url) //Make sure that the route is the same with your configured route for your HUB
            //    .Build();

            var connectionFactory = this is SignalRTextChannel
                ? new SignalRTextConnectionFactory(LogEventListener)
                : new SignalRConnectionFactory(LogEventListener);

            var protocol = new JsonHubProtocol();

            var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(new LoggerProvider(LogEventListener)));

            Connection = new HubConnection(connectionFactory, protocol, new UriEndPoint(Url), this, loggerFactory);

            Connection.Reconnected += x =>
            {
                Console.WriteLine(x); 
                return Task.CompletedTask;
            };

            // Subscribe to event
            Connection.Closed += ex =>
            {
                if (ex == null)
                {
                    //Trace.WriteLine("Connection terminated");
                    ConnectionState = SignalRChannelState.Disconnected;
                }
                else
                {
                    //Trace.WriteLine($"Connection terminated with error: {ex.GetType()}: {ex.Message}");
                    ConnectionState = SignalRChannelState.Disconnected; //.Faulted;
                }

                return Task.CompletedTask;
            };


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
        private void SetState()
        {
            switch (ConnectionState)
            {
                case SignalRChannelState.Off:
                    break;

                case SignalRChannelState.Connected:
                    break;

                case SignalRChannelState.Connecting:
                    break;

                case SignalRChannelState.Disconnected:
                    break;

                case SignalRChannelState.Reconnecting:
                    break;

                case SignalRChannelState.Faulted:
                    break;
            }



            switch (Connection.State)
            {
                case HubConnectionState.Connected:
                    ConnectionState = SignalRChannelState.Connected;
                    ConnectionAttempts = 0;
                    break;

                case HubConnectionState.Disconnected:
                    ConnectionState = SignalRChannelState.Disconnected;
                    break;

                case HubConnectionState.Connecting:
                    ConnectionState = SignalRChannelState.Connecting;
                    try
                    {
                        ++ConnectionAttempts;
                    }
                    catch
                    {
                        ConnectionAttempts = 0;
                    }
                    break;

                case HubConnectionState.Reconnecting:
                    ConnectionState = SignalRChannelState.Reconnecting;
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
