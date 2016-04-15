using System;

namespace Xamarin.Forms.OAuth
{
    public class OAuthAccessToken
    {
        internal OAuthAccessToken(string token, DateTime expires)
        {
            Token = token;
            Expires = expires;
        }

        internal OAuthAccessToken(string token, string refreshToken, DateTime expires)
            : this(token, expires)
        {
            RefreshToken = refreshToken;
        }

        public string Token { get; private set; }
        public string RefreshToken { get; private set; }
        public DateTime Expires { get; private set; }
    }
}
