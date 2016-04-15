namespace Xamarin.Forms.OAuth.Providers
{
    public class AmazonOAuthProvider : OAuthProvider
    {
        public AmazonOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Amazon";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://www.amazon.com/ap/oa";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.amazon.com/user/profile";
            }
        }

        protected override string[] MandatoryScopes
        {
            get
            {
                return new[] { "profile" };
            }
        }

        internal override string IdPropertyName
        {
            get
            {
                return "user_id";
            }
        }
    }
}
