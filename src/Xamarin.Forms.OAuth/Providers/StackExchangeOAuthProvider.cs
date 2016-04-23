using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class StackExchangeOAuthProvider : OAuthProvider
    {
        // Does not support token refresh. Tokens are refreshed on usage.
        // Define 'no_expiry' scope for a permanet token
        private const string _redirectUrl = "https://stackexchange.com/oauth/login_success";

        internal StackExchangeOAuthProvider(string clientId, string clientSecret, string redirectUrl, string key, string site, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "StackExchange",
                "https://stackexchange.com/oauth" + (string.IsNullOrEmpty(redirectUrl) ? "/dialog" : string.Empty),
                "https://stackexchange.com/oauth/access_token",
                "https://api.stackexchange.com/2.0/me",
                clientId,
                clientSecret,
                redirectUrl ?? _redirectUrl,
                scopes)
            {
                AuthorizationType = AuthorizationType.Explicit,
                TokenResponseSerialization = TokenResponseSerialization.UrlEncoded,
                ExpiresParameter = "expires",
                IncludeRedirectUrlInTokenRequest = true,
                ResourceQueryParameters = new[]
                {
                    new KeyValuePair<string, string>("key", key),
                    new KeyValuePair<string, string>("site", site)
                }
            })
        {
        }

        internal StackExchangeOAuthProvider(string clientId, string clientSecret, string key, string site, params string[] scopes)
         : this(clientId, clientSecret, null, key, site, scopes)
        { }

        internal override AccountData ReadAccountData(string json)
        {
            var jObject = JObject.Parse(json);
            var data = jObject["items"][0] as JObject;

            return new AccountData(
                data.GetStringValue("user_id"),
                data.GetStringValue("display_name"));
        }
    }
}
