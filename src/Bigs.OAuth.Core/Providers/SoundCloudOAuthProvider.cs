namespace Bigs.OAuth.Providers
{
    public sealed class SoundCloudOAuthProvider : OAuthProvider
    {
        internal SoundCloudOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes) 
            : base(new OAuthProviderDefinition(
                "SoundCloud",
                "https://soundcloud.com/connect",
                "https://api.soundcloud.com/oauth2/token",
                "http://api.soundcloud.com/me",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                RequiresCode = true,
                IncludeRedirectUrlInTokenRequest = true,
                TokeUrlParameter = "oauth_token",
                GraphNameProperty = "full_name"
            })
        {
        }
    }
}