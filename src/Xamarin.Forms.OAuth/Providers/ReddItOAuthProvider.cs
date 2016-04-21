using System;
using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class ReddItOAuthProvider : OAuthProvider
    {
        //TODO: add duration parameter in Authorize to get refresh_token
        internal ReddItOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "ReddIt",
                "https://www.reddit.com/api/v1/authorize",
                "https://www.reddit.com/api/v1/access_token",
                null,
                "https://oauth.reddit.com/api/v1/me",
                clientId,
                null,
                redirectUrl,
                scopes)
            {
                MandatoryScopes = new[] { "identity" },
                IncludeStateInAuthorize = true,
                IncludeRedirectUrlInTokenRequest = true,
                ExcludeClientIdInTokenRequest = true,
                TokenType = TokenType.Bearer,
                TokenAuthorizationHeaders = new[] 
                {
                    BuildBasicAuthenticationHeader(clientId, null)
                }
            })
        { }

        internal override IEnumerable<KeyValuePair<string, string>> ResourceHeaders(OAuthAccessToken token)
        {
            return base.ResourceHeaders(token).Union(new[]
            {
                //TODO: build User-Agent according to documentation
                //https://github.com/reddit/reddit/wiki/API
                new KeyValuePair<string, string>("User-Agent", Guid.NewGuid().ToString()),
            });
        }
    }
}
