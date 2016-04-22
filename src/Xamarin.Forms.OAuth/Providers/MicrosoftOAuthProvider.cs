namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class MicrosoftOAuthProvider : OAuthProvider
    {
        // For a refresh to be returned 'wl.offline_access' scope needs to be defined
        internal MicrosoftOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Microsoft",
                "https://login.live.com/oauth20_authorize.srf",
                "https://login.live.com/oauth20_token.srf",
                "https://apis.live.net/v5.0/me",
                clientId,
                null,
                redirectUrl,
                scopes)
            {
                AuthorizationType = string.IsNullOrEmpty(clientSecret) ? AuthorizationType.Implicit : AuthorizationType.Code,
                MandatoryScopes = new[] { "wl.signin", "wl.basic" }
            })
        { }

        internal MicrosoftOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : this(clientId, null, redirectUrl, scopes)
        { }
    }
}