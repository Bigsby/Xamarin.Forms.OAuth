using System.Collections.Generic;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class GitHubOAuthProvider : OAuthProvider
    {
        private static IEnumerable<KeyValuePair<string, string>> _headers = new[] { new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0") };

        internal GitHubOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "GitHub",
                "https://github.com/login/oauth/authorize",
                "https://github.com/login/oauth/access_token",
                "https://api.github.com/user",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                TokenResponseSerialization = TokenResponseSerialization.Forms,
                TokenAuthorizationHeaders = _headers
            })
        { }

        internal override IEnumerable<KeyValuePair<string, string>> ResourceHeaders(OAuthAccessToken token)
        {
            //TODO: Find correct way in documentation and remove this workaround
            return _headers;
        }
    }
}
