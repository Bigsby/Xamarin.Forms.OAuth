namespace Xamarin.Forms.OAuth.Providers
{
    public class BoxOAuthProvider : OAuthProvider
    {
        internal BoxOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Box";
            }
        }

        protected override string AuthoizationUrl
        {
            get
            {
                return "https://account.box.com/api/oauth2/authorize";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.box.com/2.0/users";
            }
        }
    }
}
