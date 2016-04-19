using System.Collections.Generic;

namespace Xamarin.Forms.OAuth.Providers
{
    public class GitHubOAuthProvider : OAuthProvider
    {
        internal GitHubOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(clientId, clientSecret, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "GitHub";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://github.com/login/oauth/authorize";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.github.com/user";
            }
        }

        protected override bool RequiresCode
        {
            get
            {
                return true;
            }
        }

        internal override string TokenUrl
        {
            get
            {
                return "https://github.com/login/oauth/access_token";
            }
        }

        protected override TokenResponseSerialization TokenResponseSerialization
        {
            get
            {
                return TokenResponseSerialization.Forms;
            }
        }

        internal override IEnumerable<KeyValuePair<string, string>> ResourceHeaders(OAuthAccessToken token)
        {
            //TODO: Find correct way in documentation and remove this workaround
            return new[] { new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0") };
        }
    }
}
