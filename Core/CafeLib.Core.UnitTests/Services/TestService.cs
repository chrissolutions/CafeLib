namespace CafeLib.Core.UnitTests.Services
{
    public interface ITestService
    {
        string Test();
    }

    public class TestService : ITestService
    {
        public string Test()
        {
            return "Kilroy is here!";
        }
    }
}