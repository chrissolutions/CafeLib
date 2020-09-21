using System.Threading.Tasks;
using CafeLib.Data.UnitTest.Identity;
using CafeLib.Data.UnitTest.IdentityAccess;
using CafeLib.Data.UnitTest.Models;
using Xunit;

namespace CafeLib.Data.UnitTest
{
    public class DataAccessTests
    {
        private readonly IdentityDatabase _database;

        public DataAccessTests()
        {
            _database = new IdentityDatabase();                
        }

        [Fact]
        public async Task IdentityUser_Test()
        {
            var login = new LoginModel
            {
                UserName = "alice",
                Password = "My long 123$ password",
                EmailAddress = "AliceSmith@email.com",
                RememberMe = true
            };

            var storage = _database.GetIdentityStorage();
            var user = await storage.FindUserByUserName<IdentityUser>(login.UserName);
            Assert.NotNull(user);
            Assert.Equal(login.UserName, user.UserName);
        }
    }
}
