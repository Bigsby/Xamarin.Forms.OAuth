namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class FacebookOAuthProvider : OAuthProvider
    {
        private const string _tokenUrl = "https://graph.facebook.com/v2.3/oauth/access_token";
        // This URL is provided in documentation as mandatory for desktop apps (Manually retrieved tokens)
        private const string _redirectUrl = "https://www.facebook.com/connect/login_success.html";

        internal FacebookOAuthProvider(string appId, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Facebook",
                "https://www.facebook.com/dialog/oauth",
                _tokenUrl,
                "https://graph.facebook.com/v2.5/me",
                appId,
                null,
                _redirectUrl,
                scopes
                )
            {
                AuthorizationType = AuthorizationType.Token,
                RefreshTokenUrl = _tokenUrl
            })
        { }
    }
}