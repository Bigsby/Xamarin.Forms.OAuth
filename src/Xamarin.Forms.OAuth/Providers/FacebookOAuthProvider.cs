namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class FacebookOAuthProvider : OAuthProvider
    {
        private const string _authorizationUrl = "https://www.facebook.com/dialog/oauth?response_type=token&";
        private const string _redirectUrl = "https://www.facebook.com/connect/login_success.html";

        internal FacebookOAuthProvider(string appId)
            : base(appId, 
                  _redirectUrl)
        { }

        protected override string AuthoizationUrl
        {
            get
            {
                return "https://www.facebook.com/dialog/oauth?response_type=token&";
            }
        }

        public override string Name
        {
            get
            {
                return "Facebook";
            }
        }

        protected override string GraphUrl
        {
            get { return "https://graph.facebook.com/v2.5/me"; }
        }
    }
}
