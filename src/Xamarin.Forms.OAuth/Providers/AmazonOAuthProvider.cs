namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class AmazonOAuthProvider : OAuthProvider
    {
        internal AmazonOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Amazon",
                "https://www.amazon.com/ap/oa",
                "https://api.amazon.com/auth/o2/token",
                "https://api.amazon.com/user/profile",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                AuthorizationType = string.IsNullOrEmpty(clientSecret) ? AuthorizationType.Implicit : AuthorizationType.Explicit,
                RefreshesToken = !string.IsNullOrEmpty(clientSecret),
                MandatoryScopes = new[] { "profile" },
                IncludeRedirectUrlInTokenRequest = true,
                TokenType = string.IsNullOrEmpty(clientSecret) ? TokenType.Url : TokenType.Bearer,
                GraphIdProperty = "user_id",
            })
        { }

        internal AmazonOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : this(clientId, null, redirectUrl, scopes)
        { }
    }
}
