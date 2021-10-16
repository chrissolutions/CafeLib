using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CafeLib.Authorization.Tokens
{
    public class TokenBuilder : ITokenBuilder
    {
        internal const string DefaultIssuer = "issuer";
        internal const string DefaultAudience = "audience";
        private readonly SecurityTokenDescriptor _descriptor = new SecurityTokenDescriptor();

        public TokenBuilder()
        {
            AddIssuer(DefaultIssuer);
            AddAudience(DefaultAudience);
        }

        public Token Build()
        {
            var claims = _descriptor.Claims.Select(x => new Claim(x.Key, x.Value.ToString()));
            var token = new JwtSecurityToken(_descriptor.Issuer, _descriptor.Audience, claims, expires: _descriptor.Expires, signingCredentials: _descriptor.SigningCredentials);
            return new Token(token);
        }

        public ITokenBuilder AddIssuer(string issuer)
        {
            _descriptor.Issuer = issuer;
            return this;
        }

        public ITokenBuilder AddAudience(string audience)
        {
            _descriptor.Audience = audience;
            return this;
        }

        public ITokenBuilder AddClaims(ClaimCollection claims)
        {
            _descriptor.Claims = claims.ToDictionary(x => x.Key, x => (object)x.Value);
            return this;
        }

        public ITokenBuilder AddSecret(string secret)
        {
            _descriptor.SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha256);
            return this;
        }

        public ITokenBuilder Expires(DateTime expiry)
        {
            _descriptor.Expires = expiry;
            return this;
        }
    }
}
