namespace Xamarin.Forms.OAuth.Providers
{
    public class ReddItOAuthProvider : OAuthProvider
    {
        internal ReddItOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "ReddIt";
            }
        }

        protected override string AuthoizationUrl
        {
            get
            {
                return "https://www.reddit.com/api/v1/authorize";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://www.reddit.com/api/v1/me";
            }
        }
    }
}
