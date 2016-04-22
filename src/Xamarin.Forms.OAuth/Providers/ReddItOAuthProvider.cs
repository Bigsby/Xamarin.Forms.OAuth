using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class ReddItOAuthProvider : OAuthProvider
    {
        //TODO: build User-Agent according to documentation
        //https://github.com/reddit/reddit/wiki/API
        private static string _userAgentId = Guid.NewGuid().ToString();

        //TODO: add duration parameter in Authorize to get refresh_token
        internal ReddItOAuthProvider(string clientId, string clientSecret, string redirectUrl, bool implicitFlow, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "ReddIt",
                "https://www.reddit.com/api/v1/authorize",
                "https://www.reddit.com/api/v1/access_token",
                "https://oauth.reddit.com/api/v1/me",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                AuthorizationType = implicitFlow ? AuthorizationType.Implicit : AuthorizationType.Code,
                AuthorizeCustomQueryParameters = AuthorizaQueryParameters(clientSecret),
                MandatoryScopes = new[] { "identity" },
                IncludeStateInAuthorize = true,
                IncludeRedirectUrlInTokenRequest = true,
                IncludeRedirectUrlInRefreshTokenRequest = true,
                RefreshesToken = !string.IsNullOrEmpty(clientSecret),
                ExcludeClientIdAndSecretInTokenRefresh = true,
                ExcludeClientIdInTokenRequest = true,
                ExcludeClientSecretInTokenRequest = true,
                TokenType = TokenType.Bearer,
                TokenAuthorizationHeaders = new[] { BuildBasicAuthenticationHeader(clientId, clientSecret) },
                RefreshTokenAuthorizationHeaders = new[] { BuildBasicAuthenticationHeader(clientId, clientSecret) }
            })
        { }

        internal ReddItOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : this(clientId, clientSecret, redirectUrl, false, scopes)
        { }

        internal ReddItOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            :this(clientId, null, redirectUrl, false, scopes)
        { }

        internal override IEnumerable<KeyValuePair<string, string>> CustomResourceHeaders(OAuthAccessToken token)
        {
            return base.CustomResourceHeaders(token).Union(new[]
            {
                new KeyValuePair<string, string>("User-Agent", _userAgentId),
            });
        }

        private static IEnumerable<KeyValuePair<string, string>> AuthorizaQueryParameters(string clientSecret)
        {
            return string.IsNullOrEmpty(clientSecret) ?
                new KeyValuePair<string, string>[0]
                :
                new[] { new KeyValuePair<string, string>("duration", "permanent") };
        }
    }
}
