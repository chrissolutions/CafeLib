using CafeLib.Core.Extensions;
using CafeLib.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using CafeLib.Core.Support;
using Xunit;

namespace CafeLib.Core.UnitTests
{
    public class LogEventTests
    {
        private static void InfoLogTestLogListener(LogEventMessage logEventMessage)
        {
            Assert.Equal("3: Log event message", logEventMessage.EventInfo.ToString());
            Assert.Equal("Hello World", logEventMessage.Message);
        }

        private static void TestLoggerFactoryListener(LogEventMessage eventMessage)
        {
            Assert.Equal(typeof(HttpClient).FullName, eventMessage.Category);
            Assert.Equal(3, eventMessage.EventInfo.Id);
            Assert.Equal("TestEvent", eventMessage.EventInfo.Name);
            Assert.Equal(2, eventMessage.MessageInfo.Count);
            Assert.Equal(20, eventMessage.MessageInfo["tag"]);
            Assert.Equal(40, eventMessage.MessageInfo["state"]);
            Assert.Equal("Message 20 for state 40", eventMessage.Message);
        }

        [Fact]
        public void StringToLogEventInfoTest()
        {
            var logEventInfo = new LogEventInfo("3: Log event message");
            Assert.Equal(3, logEventInfo.Id);
            Assert.Equal("Log event message", logEventInfo.Name);
        }

        [Fact]
        public void LoggerTest()
        {
            var factory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddProvider(new LoggerProvider(TestLoggerFactoryListener));
            });

            factory.CreateLogger<HttpClient>();

            var logger = (ILogger)new Logger<HttpClient>(factory);

            logger.Log(LogLevel.Information,
                new EventId(3, "TestEvent"),
                new { tag = 20, state = 40 },
                null,
                (o, e) => "Message {tag} for state {state}".Render(o));
        }

        [Fact]
        public void LoggerInfoLevelTest()
        {
            var factory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddProvider(new LoggerProvider(x =>
                    {
                        Assert.Equal(typeof(HttpClient).FullName, x.Category);
                        Assert.Equal(ErrorLevel.Info, x.ErrorLevel);
                        Assert.Equal("Hello World", x.Message);
                    }));
            });

            factory.CreateLogger<HttpClient>();

            var logger = (ILogger)new Logger<HttpClient>(factory);

            logger.LogInformation("Hello World");
        }

        [Fact]
        public void LoggerInfoExtensionTest()
        {
            var factory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddProvider(new LoggerProvider(x =>
                    {
                        Assert.Equal(typeof(HttpClient).FullName, x.Category);
                        Assert.Equal(ErrorLevel.Info, x.ErrorLevel);
                        Assert.Equal("Hello World", x.Message);
                    }));
            });

            factory.CreateLogger<HttpClient>();

            var logger = new Logger<HttpClient>(factory);

            logger.Info("Hello World");
        }

        [Fact]
        public void LogEventWriterTest()
        {
            var writer = new LogEventWriter("UnitTest", InfoLogTestLogListener);
            writer.Info(new LogEventInfo(id: 3, name: "Log event message"), "Hello World");
        }

        [Fact]
        public void LogEventWriterLogEventMessageTest()
        {
            var writer = new LogEventWriter(typeof(HttpClient).FullName, TestLoggerFactoryListener);
            writer.LogMessage(ErrorLevel.Info,
                new LogEventInfo(id: 3, name: "TestEvent"),
                new { tag = 20, state = 40 },
                null,
                (o, e) => "Message {tag} for state {state}".Render(o));

        }
    }
}
