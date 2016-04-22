namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class GoogleOAuthProvider : OAuthProvider
    {
        internal GoogleOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Google",
                "https://accounts.google.com/o/oauth2/v2/auth",
                "https://www.googleapis.com//oauth2/v4/token",
                "https://www.googleapis.com/plus/v1/people/me",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                AuthorizationType = string.IsNullOrEmpty(clientSecret) ? AuthorizationType.Implicit : AuthorizationType.Code,
                MandatoryScopes = new[] { "profile" },
                IncludeRedirectUrlInTokenRequest = true,
                TokenType = string.IsNullOrEmpty(clientSecret) ? TokenType.Url : TokenType.Bearer,
                RefreshesToken = true,
                GraphNameProperty = "displayName"
            })
        { }

        internal GoogleOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : this(clientId, null, redirectUrl, scopes)
        { }
    }
}