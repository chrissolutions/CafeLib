using System;

namespace CafeLib.Authorization.Tokens
{
    public interface ITokenBuilder : IBuilder<Token>
    {
        ITokenBuilder AddIssuer(string issuer);
        ITokenBuilder AddAudience(string audience);
        ITokenBuilder AddClaims(ClaimCollection claims);
        ITokenBuilder AddSecret(string secret);
        ITokenBuilder Expires(DateTime expiry);
    }
}
