namespace Xamarin.Forms.OAuth.Providers
{
    public class InstagramOAuthProvider : OAuthProvider
    {
        internal InstagramOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Instagram";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://api.instagram.com/oauth/authorize";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.instagram.com/v1/users/self";
            }
        }
    }
}
