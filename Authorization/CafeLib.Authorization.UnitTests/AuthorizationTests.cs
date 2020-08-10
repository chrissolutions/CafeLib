using System;
using CafeLib.Authorization.Tokens;
using CafeLib.Core.Extensions;
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
            var expires = DateTime.UtcNow.AddHours(3);
            var tokenBuilder = new TokenBuilder()
                .AddIssuer(TestIssuer)
                .AddAudience(TestAudience)
                .AddClaims(_claimCollection)
                .Expires(expires);

            var token = tokenBuilder.Build();

            Assert.NotNull(token);
            Assert.Equal(TestIssuer, token.Issuer);
            Assert.Equal(expires.Truncate(TimeSpan.FromSeconds(1)), token.Expires);
        }
    }
}
