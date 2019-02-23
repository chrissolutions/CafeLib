using CafeLib.Core.Extensions;
using CafeLib.Core.Logging;

namespace CafeLib.Core.UnitTests
{
    public class TestLogFactory : LogFactory<TestLogHandler>
    {
        public TestLogFactory(ILogEventMessenger messenger = null)
            : base(messenger)
        {
        }

        public TestLogFactory(NonNullable<string> category, ILogEventMessenger messenger)
            : base(category, messenger)
        {
        }
    }
}