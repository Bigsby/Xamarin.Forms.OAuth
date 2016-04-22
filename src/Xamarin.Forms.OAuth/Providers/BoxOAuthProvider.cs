namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class BoxOAuthProvider : OAuthProvider
    {
        internal BoxOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Box",
                "https://account.box.com/api/oauth2/authorize",
                "https://api.box.com/oauth2/token",
                "https://api.box.com/2.0/users/me",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                RefreshesToken = true
            })
        { }
    }
}
