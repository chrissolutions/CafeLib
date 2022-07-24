using System.Threading.Tasks;
using CafeLib.Authorization.Identity;
using CafeLib.Authorization.UnitTests.IdentityAccess;
using Xunit;

namespace CafeLib.Authorization.UnitTests
{
    public class IdentityAccessTests
    {
        private readonly IdentityDatabase _identityDatabase;

        public IdentityAccessTests()
        {
            _identityDatabase = new IdentityDatabase();
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

            var storage = _identityDatabase.GetStorage();
            var user = await storage.FindUserByUserName<IdentityUser>(login.UserName);
            Assert.NotNull(user);
            Assert.Equal(login.UserName, user.UserName);
        }
    }
}
