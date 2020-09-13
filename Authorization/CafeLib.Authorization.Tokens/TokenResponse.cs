using System;

namespace CafeLib.Authorization.Tokens
{
    public class TokenResponse
    {
        public Token Token { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}
