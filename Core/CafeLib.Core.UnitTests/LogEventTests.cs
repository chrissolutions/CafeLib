using CafeLib.Core.Extensions;
using CafeLib.Core.Logging;
using CafeLib.Core.UnitTests.Logging;
using Microsoft.Extensions.Logging;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class LogEventTests
    {
        [Fact]
        public void LogEventWriterTest()
        {
            var writer = new LogEventWriter("UnitTest", InfoLogTestLogListener);
            writer.Info(new LogEventInfo(id: 3, name: "Log event message"), "Hello World");
        }

        private static void InfoLogTestLogListener(LogEventMessage logEventMessage)
        {
            Assert.Equal("3: Log event message", logEventMessage.EventInfo.ToString());
            Assert.Equal("Hello World", logEventMessage.Message);
        }

        [Fact]
        public void StringToLogEventInfoTest()
        {
            var logEventInfo = new LogEventInfo("3: Log event message");
            Assert.Equal(3, logEventInfo.Id);
            Assert.Equal("Log event message", logEventInfo.Name);
        }

        [Fact]
        public void LogFactoryTest()
        {
            var messenger = new TestLogFactoryMessenger("testFactory", TestLoggerFactoryListener);

            messenger.Logger.Log(LogLevel.Information, 
                        new EventId(3, "TestEvent"),
                        new {tag = 20, state = 40},
                        null, 
                        (o, e) => "Message {tag} for state {state}".Render(o));
        }

        [Fact]
        public void LogProviderTest()
        {
            var factory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddProvider(new LogProvider<TestLogSender>(new TestLogReceiver(TestLoggerFactoryListener)));
            });

            var logger = factory.CreateLogger<TestLogSender>();
            logger.Log(LogLevel.Information,
                        new EventId(3, "TestEvent"),
                        new { tag = 20, state = 40 },
                        null,
                        (o, e) => "Message {tag} for state {state}".Render(o));
        }

        private static void TestLoggerFactoryListener(LogEventMessage logEventMessage)
        {
            var eventMessage = (TestLogEventMessage)logEventMessage;
            Assert.Equal(3, eventMessage.EventInfo.Id);
            Assert.Equal("TestEvent", eventMessage.EventInfo.Name);
            Assert.Equal(2, eventMessage.MessageInfo.Count);
            Assert.Equal(20, eventMessage.MessageInfo["tag"]);
            Assert.Equal(40, eventMessage.MessageInfo["state"]);
            Assert.Equal("Message 20 for state 40", logEventMessage.Message);
        }

        [Fact]
        public void LogAdapterTest()
        {
            var factory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddProvider(new LogAdapterProvider<TestLogAdapter>(TestLoggerFactoryListener));
            });

            var logger = factory.CreateLogger<TestLogAdapter>();

            logger.Log(LogLevel.Information,
                new EventId(3, "TestEvent"),
                new { tag = 20, state = 40 },
                null,
                (o, e) => "Message {tag} for state {state}".Render(o));
        }
    }
}
