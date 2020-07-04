using System;
using CafeLib.Core.Logging;
using Microsoft.Extensions.Logging;

namespace CafeLib.Core.UnitTests.Logging
{
    //public class TestLogFactoryMessenger : ILogEventReceiver
    //{
    //    private readonly Action<LogEventMessage> _listener;

    //    public TestLogFactoryMessenger(string category, Action<LogEventMessage> listener)
    //    {
    //        _listener = listener;
    //        var factory = new TestLogFactory(category, this);
    //        Logger = factory.CreateLogger<TestLogSender>();
    //    }

    //    public ILogger Logger { get; }

    //    public void LogMessage<T>(T message) where T : LogEventMessage
    //    {
    //        _listener?.Invoke(message);
    //    }
    //}
}