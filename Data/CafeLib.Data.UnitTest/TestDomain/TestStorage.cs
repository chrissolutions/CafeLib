using CafeLib.Data.Persistence;

namespace CafeLib.Data.UnitTest.TestDomain
{
    public class TestStorage : Storage<TestContext>
    {
        public TestStorage(string connectionUri) : base(connectionUri)
        {
        }
    }
}
