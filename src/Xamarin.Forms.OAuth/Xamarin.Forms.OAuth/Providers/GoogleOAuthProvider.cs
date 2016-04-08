namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class GoogleOAuthProvider : OAuthProvider
    {
        private const string _authorizationUrl = "https://accounts.google.com/o/oauth2/v2/auth?response_type=token&scope=profile&";

        public GoogleOAuthProvider(string clientId, string redirectUrl)
            : base(clientId, redirectUrl)
        { }

        protected override string AuthoizationUrl
        {
            get
            {
                return "https://accounts.google.com/o/oauth2/v2/auth?response_type=token&scope=profile&";
            }
        }

        internal override string NamePropertyName
        {
            get
            {
                return "displayName";
            }
        }

        public override string Name
        {
            get
            {
                return "Google";
            }
        }

        protected override string GraphUrl
        {
            get { return "https://www.googleapis.com/plus/v1/people/me"; }
        }
    }
}
