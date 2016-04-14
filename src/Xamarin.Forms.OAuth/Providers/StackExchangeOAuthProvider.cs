namespace Xamarin.Forms.OAuth.Providers
{
    public class StackExchangeOAuthProvider : OAuthProvider
    {
        private const string _redirectUrl = "https://stackexchange.com/oauth/login_success";
        public StackExchangeOAuthProvider(string clientId, params string[] scopes)
            : base(clientId, _redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "StackExchange";
            }
        }

        protected override string AuthoizationUrl
        {
            get
            {
                return "https://stackexchange.com/oauth/dialog";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.stackexchange.com//users/me";
            }
        }
    }
}
