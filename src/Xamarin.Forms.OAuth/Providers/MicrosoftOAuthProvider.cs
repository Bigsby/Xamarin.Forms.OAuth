using System.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class MicrosoftOAuthProvider : OAuthProvider
    {
        // For a refresh to be returned 'wl.offline_access' scope needs to be defined
        internal MicrosoftOAuthProvider(string clientId, string clientSecret, string redirectUrl, bool offlineAccess, params string[] scopes)
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
                AuthorizationType = string.IsNullOrEmpty(clientSecret) ? AuthorizationType.Implicit : AuthorizationType.Explicit,
                TokenType = string.IsNullOrEmpty(clientSecret) ? TokenType.Url : TokenType.Bearer,
                MandatoryScopes = GetMandatoryScoptes(offlineAccess),
                IncludeRedirectUrlInTokenRequest = true,
                IncludeRedirectUrlInRefreshTokenRequest = true,
                RefreshesToken = !string.IsNullOrEmpty(clientSecret)
            })
        { }

        internal MicrosoftOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : this(clientId, clientSecret, redirectUrl, false, scopes)
        { }

        internal MicrosoftOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : this(clientId, null, redirectUrl, false, scopes)
        { }

        private static string[] GetMandatoryScoptes(bool offlineAccess)
        {
            return new[] { "wl.signin", "wl.basic" }.Union(offlineAccess ? new[] { "wl.offline_access" } : new string[0]).ToArray();
        }
    }
}