using System;
using System.Threading;
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

        private int _connectionAttempts;

        #endregion

        #region Automatic Properties
        protected MethodBridge Bridge { get; set; }

        internal HubConnection Connection { get; private set; }

        protected ILogger Logger { get; }

        public Uri Url { get; }

        public SignalRChannelState ConnectionState { get; private set; }

        public int ConnectionAttempts => _connectionAttempts;

        public event Action<SignalRChangedMessage> Changed;

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

            Advised += x =>
            {
                if (!(x is RunnerEventMessage message) || message.ErrorLevel == ErrorLevel.Ignore) return;
                LogEventListener(new LogEventMessage(Url.ToString(), message.ErrorLevel, message.Message));
            };

            ConnectionState = SignalRChannelState.Off;
        }

        #endregion

        #region Methods

        public override async Task Start()
        {
            SendChangedEvent();
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
                await VerifyConnection();
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
            Logger.LogMessage(message.ErrorLevel, message.EventInfo, message.Message, message.Exception);
        }

        /// <summary>
        /// Connect the web channel.
        /// </summary>
        /// <returns>task</returns>
        private async Task OpenChannel()
        {
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
            Delay = DefaultDelay;
        }

        private async Task CloseChannel()
        {
            try
            {
                await Connection.StopAsync();
                await Connection.DisposeAsync();
                ConnectionState = SignalRChannelState.Off;
                SendChangedEvent();
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

        private async Task VerifyConnection()
        {
            var state = GetConnectionState();

            if (state != SignalRChannelState.Connected && state != SignalRChannelState.Connecting)
            {
                await OpenChannel();
            }

            if (state != ConnectionState)
            {
                switch (state)
                {
                    case SignalRChannelState.Connected:
                        _connectionAttempts = 0;
                        Delay = CheckConnectionInterval;
                        break;

                    case SignalRChannelState.Disconnected:
                        break;

                    case SignalRChannelState.Connecting:
                        try
                        {
                            Interlocked.Increment(ref _connectionAttempts);
                        }
                        catch
                        {
                            _connectionAttempts = 0;
                        }
                        break;

                    case SignalRChannelState.Reconnecting:
                        try
                        {
                            Interlocked.Increment(ref _connectionAttempts);
                        }
                        catch
                        {
                            _connectionAttempts = 0;
                        }
                        break;
                }

                ConnectionState = state;
                SendChangedEvent();
            }
        }

        private SignalRChannelState GetConnectionState()
        {
            switch (Connection.State)
            {
                case HubConnectionState.Connected:
                    return SignalRChannelState.Connected;

                case HubConnectionState.Disconnected:
                    return SignalRChannelState.Disconnected;

                case HubConnectionState.Connecting:
                    return SignalRChannelState.Connecting;

                case HubConnectionState.Reconnecting:
                    return SignalRChannelState.Reconnecting;

                default:
                    return SignalRChannelState.Off;
            }
        }

        private void SendChangedEvent()
        {
            Changed?.Invoke(new SignalRChangedMessage(this));
        }

        #endregion
    }
}
