namespace Xamarin.Forms.OAuth.Providers
{
    public class TwitterOAuthProvider : OAuthProvider
    {
        internal TwitterOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Twitter";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://api.twitter.com/oauth/authorize";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.twitter.com/1.1/account/verify_credentials.json";
            }
        }

        internal override string IdPropertyName
        {
            get
            {
                return "id_str";
            }
        }
    }
}
