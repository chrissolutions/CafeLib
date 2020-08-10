using System;
using System.Security.Claims;
using CafeLib.Authorization.Tokens;
using Xunit;

namespace CafeLib.Authorization.UnitTests
{
    public class AuthorizationTests
    {
        private const string TestIssuer = "testIssuer";
        private const string TestAudience = "testAudience";

        private readonly ClaimCollection _claimCollection = new ClaimCollection
        {
            {"email", "bruce.wayne@wayne.com"},
            {"firstName", "Bruce"},
            {"lastName", "Wayne"}
        };

        [Fact]
        public void BuildTokenTest()
        {
            var tokenBuilder = new TokenBuilder()
                .AddIssuer(TestIssuer)
                .AddAudience(TestAudience)
                .AddClaims(_claimCollection);

            var token = tokenBuilder.Build();

            Assert.NotNull(token);
            Assert.Equal(TestIssuer, token.Issuer);
        }
    }
}
