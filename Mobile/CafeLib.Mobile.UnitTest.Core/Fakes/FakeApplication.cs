using CafeLib.Core.IoC;
using CafeLib.Mobile.Startup;

namespace CafeLib.Mobile.UnitTest.Core.Fakes
{
    public class FakeApplication : CafeApplication
    {
        public FakeApplication(IServiceRegistry registry)
            : base(registry)
        {
        }
    }
}