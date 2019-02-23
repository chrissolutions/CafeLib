using CafeLib.Core.IoC;

namespace CafeLib.Core.UnitTests
{
    public interface ITestService
    {
        string Test();
    }

    public class TestService : ServiceBase, ITestService
    {
        public string Test()
        {
            return "Kilroy is here!";
        }
    }
}