using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace CafeLib.Authorization.Tokens
{
    public class Token
    {
        private SecurityToken _token;
        private string _tokenString;
        private readonly JwtSecurityTokenHandler _handler = new JwtSecurityTokenHandler();

        internal Token(SecurityToken token)
        {
            _token = token;
        }

        public Token(string tokenString)
        {
            _tokenString = tokenString;
        }

        public Token Validate(string issuer, string audience, string secret)
        {
            if (string.IsNullOrWhiteSpace(_tokenString)) throw new ArgumentNullException(nameof(_tokenString));
            var param = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(1),
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };

            _handler.ValidateToken(_tokenString, param, out _token);
            return this;
        }

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
    }
}
