using CafeLib.Core.Extensions;
using CafeLib.Core.Logging;

namespace CafeLib.Core.UnitTests.Logging
{
    public class TestLogFactory : LogFactory<TestLogSender>
    {
        public TestLogFactory(ILogEventReceiver receiver = null)
            : base(receiver)
        {
        }

        public TestLogFactory(NonNullable<string> category, ILogEventReceiver receiver)
            : base(category, receiver)
        {
        }
    }
}