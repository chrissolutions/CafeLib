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
        private const string TestSecret = "secretsecretsecret";

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
            Assert.Equal(expires.TruncateMilliseconds(), token.Expires);
        }

        [Fact]
        public void ValidateTokenTest()
        {
            var expires = DateTime.UtcNow.AddHours(3);
            var tokenBuilder = new TokenBuilder()
                .AddIssuer(TestIssuer)
                .AddAudience(TestAudience)
                .AddClaims(_claimCollection)
                .AddSecret(TestSecret)
                .Expires(expires);

            var token = tokenBuilder.Build();

            var validToken = token.Validate(TestIssuer, TestAudience, TestSecret);

            Assert.Equal(token.Issuer, validToken.Issuer);
        }
    }
}
