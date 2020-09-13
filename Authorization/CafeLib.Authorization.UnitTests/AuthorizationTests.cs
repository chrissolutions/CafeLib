using System;
using System.Linq;
using System.Reflection;
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
            // Arrange.
            var expires = DateTime.UtcNow.AddHours(3);
            var tokenBuilder = new TokenBuilder()
                .AddIssuer(TestIssuer)
                .AddAudience(TestAudience)
                .AddClaims(_claimCollection)
                .AddSecret(TestSecret)
                .Expires(expires);

            // Act.
            var token = tokenBuilder.Build();
            var validToken = token.Validate(TestIssuer, TestAudience, TestSecret);

            // Assert.
            Assert.Equal(token.Issuer, validToken.Issuer);
            Assert.Equal(token.Expires, validToken.Expires);
        }
        [Fact]
        public void ValidateTokenResponseTest()
        {
            // Arrange.
            var expires = DateTime.UtcNow.AddHours(3);
            var tokenBuilder = new TokenBuilder()
                .AddIssuer(TestIssuer)
                .AddAudience(TestAudience)
                .AddClaims(_claimCollection)
                .AddSecret(TestSecret)
                .Expires(expires);

            // Act.
            var token = tokenBuilder.Build();
            var result = token.TryValidate(TestIssuer, TestAudience, TestSecret, out var response);

            // Assert.
            Assert.True(result);
            Assert.NotNull(response.Token);
            Assert.Equal(token.Issuer, response.Token.Issuer);
            Assert.Equal(token.Expires, response.Token.Expires);
        }

        [Fact]
        public void ValidateClaimsTest()
        {
            // Arrange.
            var expires = DateTime.UtcNow.AddHours(3);
            var tokenBuilder = new TokenBuilder()
                .AddIssuer(TestIssuer)
                .AddAudience(TestAudience)
                .AddClaims(_claimCollection)
                .AddSecret(TestSecret)
                .Expires(expires);

            // Act.
            var token = tokenBuilder.Build();
            var validToken = token.Validate(TestIssuer, TestAudience, TestSecret);
            var claims1 = token.Claims;
            var claims2 = validToken.Claims;

            // Assert.
            claims1.ForEach((x, i) =>
            {
                var (key, value) = x;
                Assert.Equal(key, claims2.ElementAt(i).Key);
                Assert.Equal(value, claims2.ElementAt(i).Value);
            });
        }

        [Fact]
        public void ValidateTokenString()
        {
            // Arrange.
            var expires = DateTime.UtcNow.AddHours(3);
            var tokenBuilder = new TokenBuilder()
                .AddIssuer(TestIssuer)
                .AddAudience(TestAudience)
                .AddClaims(_claimCollection)
                .AddSecret(TestSecret)
                .Expires(expires);

            var token = tokenBuilder.Build();
            var tokenString = token.ToString();

            // Act.
            var testToken = new Token(tokenString);
            var result = testToken.TryValidate(TestIssuer, TestAudience, TestSecret, out var response);

            // Assert.
            Assert.True(result);
        }
    }
}
