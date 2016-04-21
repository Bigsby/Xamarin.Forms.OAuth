using System;

namespace Xamarin.Forms.OAuth
{
    public sealed class OAuthAccessToken
    {
        internal OAuthAccessToken(string token, TokenType type, DateTime? expires)
        {
            Token = token;
            Type = type;
            Expires = expires;
        }

        internal OAuthAccessToken(string token, TokenType type, string refreshToken, DateTime? expires)
            : this(token, type, expires)
        {
            RefreshToken = refreshToken;
        }

        public string Token { get; private set; }
        public string RefreshToken { get; private set; }
        public TokenType Type { get; private set; }
        public DateTime? Expires { get; private set; }
        public bool IsRefreshable { get { return !string.IsNullOrEmpty(RefreshToken); } }
    }
}