using System.Linq;

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
                clientSecret,
                redirectUrl,
                scopes)
            {
                AuthorizationType = string.IsNullOrEmpty(clientSecret) ? AuthorizationType.Implicit : AuthorizationType.Code,
                TokenType = string.IsNullOrEmpty(clientSecret) ? TokenType.Url : TokenType.Bearer,
                MandatoryScopes = GetMandatoryScoptes(clientSecret),
                IncludeRedirectUrlInTokenRequest = true,
                IncludeRedirectUrlInRefreshTokenRequest = true,
                RefreshesToken = !string.IsNullOrEmpty(clientSecret)
            })
        { }

        internal MicrosoftOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : this(clientId, null, redirectUrl, scopes)
        { }

        private static string[] GetMandatoryScoptes(string clientSecret)
        {
            return new[] { "wl.signin", "wl.basic" }.Union(string.IsNullOrEmpty(clientSecret) ? new string[0] : new[] { "wl.offline_access" }).ToArray();
        }
    }
}