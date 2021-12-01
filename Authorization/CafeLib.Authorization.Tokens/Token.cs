using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
// ReSharper disable UnusedMember.Global

namespace CafeLib.Authorization.Tokens
{
    public class Token
    {
        private readonly JwtSecurityTokenHandler _handler = new();
        private SecurityToken _token;
        private string _tokenString;

        public string Id => _token?.Id;

        public string Issuer => _token?.Issuer;

        public ClaimCollection Claims { get; private set; }

        public DateTime Expires => _token?.ValidTo ?? throw new NullReferenceException(nameof(_token));

        internal Token(SecurityToken token)
        {
            _token = token;
            Claims = GetClaims(token);
        }

        public Token(string tokenString)
        {
            _tokenString = tokenString;
        }

        /// <summary>
        /// Validate token with secret.
        /// </summary>
        /// <param name="secret"></param>
        /// <returns></returns>
        public Token Validate(string secret)
        {
            return Validate(TokenBuilder.DefaultIssuer, TokenBuilder.DefaultAudience, secret);
        }

        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="issuer">issuer</param>
        /// <param name="audience">audience</param>
        /// <param name="secret">secret</param>
        /// <returns></returns>
        public Token Validate(string issuer, string audience, string secret)
        {
            _token = GetTokenFromString(ToString(), issuer, audience, secret);
            Claims = GetClaims(_token);
            return this;
        }

        /// <summary>
        /// Validate token with secret.
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool TryValidate(string secret, out TokenResponse response)
        {
            return TryValidate(TokenBuilder.DefaultIssuer, TokenBuilder.DefaultAudience, secret, out response);
        }

        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="issuer">issuer</param>
        /// <param name="audience">audience</param>
        /// <param name="secret">secret</param>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool TryValidate(string issuer, string audience, string secret, out TokenResponse response)
        {
            try
            {
                var token = Validate(issuer, audience, secret);
                response = new TokenResponse { Token = token };
                return true;
            }
            catch (Exception ex)
            {
                response = new TokenResponse { Exception = ex, Message = ex.Message };
                return false;
            }
        }

        /// <summary>
        /// Convert token to token string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_tokenString))
            {
                if (_token != null)
                {
                    _tokenString = _handler.WriteToken(_token);
                }
                else
                {
                    throw new ArgumentNullException(nameof(_token));
                }
            }

            return _tokenString;
        }

        /// <summary>
        /// Cast to underlying SecurityToken.
        /// </summary>
        /// <param name="token"></param>
        public static implicit operator SecurityToken(Token token)
        {
            return token._token;
        }

        #region Helpers

        private static SecurityToken GetTokenFromString(string tokenString, string issuer, string audience, string secret)
        {
            var handler = new JwtSecurityTokenHandler();

            var param = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };

            handler.ValidateToken(tokenString, param, out var token);
            return token;
        }

        private static ClaimCollection GetClaims(SecurityToken token)
        {
            var jwt = (JwtSecurityToken)token;
            return new ClaimCollection(jwt.Claims.ToDictionary(x => x.Type, x => x.Value.ToString()));
        }

        #endregion
    }
}
