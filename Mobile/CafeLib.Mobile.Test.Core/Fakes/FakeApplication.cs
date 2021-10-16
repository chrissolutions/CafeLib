using CafeLib.Core.IoC;
using CafeLib.Mobile.Startup;

namespace CafeLib.Mobile.Test.Core.Fakes
{
    public class FakeApplication : CafeApplication
    {
        public FakeApplication(IServiceRegistry registry)
            : base(registry)
        {
        }
    }
}