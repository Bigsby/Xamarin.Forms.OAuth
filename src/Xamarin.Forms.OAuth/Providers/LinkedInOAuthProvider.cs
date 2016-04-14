namespace Xamarin.Forms.OAuth.Providers
{
    public class LinkedInOAuthProvider : OAuthProvider
    {
        public LinkedInOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "LinkedIn";
            }
        }

        protected override string AuthoizationUrl
        {
            get
            {
                return "https://www.linkedin.com/uas/oauth2/authorization";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.linkedin.com/v1/people/";
            }
        }
    }
}
