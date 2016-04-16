using Xamarin.Forms.OAuth.Providers;

namespace Xamarin.Forms.OAuth
{
    public static class OAuthProviders
    {
        public static OAuthProvider Amazon(string clientId, string redirectUrl, params string[] scopes)
        {
            return new AmazonOAuthProvider(clientId, redirectUrl, scopes);
        }

        public static OAuthProvider Box(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
        {
            return new BoxOAuthProvider(clientId, clientSecret, redirectUrl, scopes);
        }

        public static OAuthProvider Custom(
            string name,
            string authorizationUrl,
            string redirectUrl,
            string graphUrl,
            string clientId,
            string idProperty = null,
            string nameProperty = null,
            ImageSource logo = null,
            params string[] scopes)
        {
            return new CustomOAuthProvider(
                name,
                authorizationUrl,
                redirectUrl,
                graphUrl,
                clientId,
                idProperty,
                nameProperty,
                logo,
                scopes);
        }

        public static OAuthProvider Dropbox(string clientId, string redirectUrl, params string[] scopes)
        {
            return new DropboxOAuthProvider(clientId, redirectUrl, scopes);
        }

        public static OAuthProvider Facebook(string appId, params string[] scopes)
        {
            return new FacebookOAuthProvider(appId, scopes);
        }

        public static OAuthProvider GitHub(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
        {
            return new GitHubOAuthProvider(clientId, clientSecret, redirectUrl, scopes);
        }

        public static OAuthProvider Google(string clientId, string redirectUrl, params string[] scopes)
        {
            return new GoogleOAuthProvider(clientId, redirectUrl, scopes);
        }

        public static OAuthProvider Instagram(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
        {
            return new InstagramOAuthProvider(clientId, clientSecret, redirectUrl, scopes);
        }

        public static OAuthProvider LinkedIn(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
        {
            return new LinkedInOAuthProvider(clientId, clientSecret, redirectUrl, scopes);
        }

        public static OAuthProvider Microsoft(string clientId, string redirectUrl, params string[] scopes)
        {
            return new MicrosoftOAuthProvider(clientId, redirectUrl, scopes);
        }

        public static OAuthProvider ReddIt(string clientId, string redirectUrl, params string[] scopes)
        {
            return new ReddItOAuthProvider(clientId, redirectUrl, scopes);
        }

        public static OAuthProvider StackExchange(string clientId, string clientSecret, params string[] scopes)
        {
            return new StackExchangeOAuthProvider(clientId, clientSecret, scopes);
        }

        //public static OAuthProvider Twitter(string clientId, string redirectUrl, params string[] scopes)
        //{
        //    return new TwitterOAuthProvider(clientId, redirectUrl, scopes);
        //}
    }
}
