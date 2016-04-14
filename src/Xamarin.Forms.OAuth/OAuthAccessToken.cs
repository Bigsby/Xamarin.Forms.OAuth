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

        public string Token { get; private set; }
        public DateTime Expires { get; private set; }
    }
}
