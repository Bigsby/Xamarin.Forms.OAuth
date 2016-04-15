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

        internal override bool RequireCode
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

        internal override bool IsTokenResponseJson
        {
            get
            {
                return false;
            }
        }

        internal override string APIUserAgent
        {
            get
            {
                //TODO: allow value to come from configuration
                return "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0";
            }
        }
    }
}
