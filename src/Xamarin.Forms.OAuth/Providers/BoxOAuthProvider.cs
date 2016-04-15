namespace Xamarin.Forms.OAuth.Providers
{
    public class BoxOAuthProvider : OAuthProvider
    {
        internal BoxOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(clientId, clientSecret, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Box";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://account.box.com/api/oauth2/authorize";
            }
        }

        internal override string TokenUrl
        {
            get
            {
                return "https://api.box.com/oauth2/token";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.box.com/2.0/users/me";
            }
        }

        internal override bool RequireCode
        {
            get
            {
                return true;
            }
        }
    }
}
