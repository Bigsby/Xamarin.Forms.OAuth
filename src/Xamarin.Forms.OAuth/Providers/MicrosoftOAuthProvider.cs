namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class MicrosoftOAuthProvider : OAuthProvider
    {
        private const string _autorizationUrl = "https://login.live.com/oauth20_authorize.srf?response_type=token&scope=wl.signin,wl.basic&";
        public MicrosoftOAuthProvider(string clientId, string redirectUrl)
            : base(clientId, redirectUrl)
        { }

        public override string Name
        {
            get
            {
                return "Microsoft";
            }
        }

        protected override string AuthoizationUrl
        {
            get
            {
                return "https://login.live.com/oauth20_authorize.srf?response_type=token&scope=wl.signin,wl.basic&";
            }
        }

        protected override string GraphUrl
        {
            get { return "https://apis.live.net/v5.0/me"; }
        }
    }
}
