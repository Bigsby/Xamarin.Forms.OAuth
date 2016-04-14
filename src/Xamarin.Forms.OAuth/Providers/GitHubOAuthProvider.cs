namespace Xamarin.Forms.OAuth.Providers
{
    public class GitHubOAuthProvider : OAuthProvider
    {
        internal GitHubOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "GitHub";
            }
        }

        protected override string AuthoizationUrl
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
    }
}
