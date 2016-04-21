namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class MicrosoftOAuthProvider : OAuthProvider
    {
        internal MicrosoftOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Microsoft",
                "https://login.live.com/oauth20_authorize.srf",
                null,
                null,
                "https://apis.live.net/v5.0/me",
                clientId,
                null,
                redirectUrl,
                scopes)
            {
                AuthorizationType = AuthorizationType.Token,
                MandatoryScopes = new[] { "wl.signin", "wl.basic" }
            })
        { }
    }
}