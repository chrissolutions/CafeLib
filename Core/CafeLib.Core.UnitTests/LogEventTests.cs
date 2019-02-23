using System.Threading.Tasks;
using CafeLib.Core.Extensions;
using CafeLib.Core.IoC;
using CafeLib.Core.Logging;
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
            var messenger = new TestLogFactoryMessenger("testfactory", TestLoggerFactoryListener);

            messenger.Logger.Log(LogLevel.Information, 
                        new EventId(3, "TestEvent"),
                        new {tag = 20, state = 40},
                        null, 
                        (o, e) => "Message {tag} for state {state}".Render(o));
        }

        private static void TestLoggerFactoryListener(LogEventMessage logEventMessage)
        {
            var eventMessage = (TestLogEventMessage)logEventMessage;
            Assert.Equal(2, eventMessage.MessageInfo.Count);
            Assert.Equal(20, eventMessage.MessageInfo["tag"]);
            Assert.Equal(40, eventMessage.MessageInfo["state"]);
            Assert.Equal("Message 20 for state 40", logEventMessage.Message);
        }

        [Fact]
        public void ServiceProviderTest()
        {
            ServiceProvider.InitAsync(async () =>
            {
                ServiceProvider.Register<ITestService>(p => new TestService());
                await Task.CompletedTask;
            }).Wait();

            var propertyService = ServiceProvider.Resolve<IPropertyService>();
            Assert.NotNull(propertyService);
            propertyService.SetProperty("name", "Kilroy");
            Assert.Equal("Kilroy", propertyService.GetProperty<string>("name"));

            var testService = ServiceProvider.Resolve<ITestService>();
            Assert.Equal("Kilroy is here!", testService.Test());
        }
    }
}
