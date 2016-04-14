namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class FacebookOAuthProvider : OAuthProvider
    {
        // This URL is provided in documentation as mandatory for desktop apps (Manually retrieved tokens)
        private const string _redirectUrl = "https://www.facebook.com/connect/login_success.html";

        internal FacebookOAuthProvider(string appId, params string[] scopes)
            : base(appId, _redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Facebook";
            }
        }

        protected override string AuthoizationUrl
        {
            get
            {
                return "https://www.facebook.com/dialog/oauth";
            }
        }

        protected override string GraphUrl
        {
            get { return "https://graph.facebook.com/v2.5/me"; }
        }
    }
}