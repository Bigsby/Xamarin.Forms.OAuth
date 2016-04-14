namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class MicrosoftOAuthProvider : OAuthProvider
    {
        public MicrosoftOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Microsoft";
            }
        }

        protected override string[] MandatoryScopes
        {
            get
            {
                return new[] { "wl.signin", "wl.basic" };
            }
        }

        protected override string AuthoizationUrl
        {
            get
            {
                return "https://login.live.com/oauth20_authorize.srf";
            }
        }

        protected override string GraphUrl
        {
            get { return "https://apis.live.net/v5.0/me"; }
        }
    }
}