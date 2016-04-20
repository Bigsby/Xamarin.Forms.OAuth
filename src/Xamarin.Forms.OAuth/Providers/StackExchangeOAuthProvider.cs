using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class StackExchangeOAuthProvider : OAuthProvider
    {
        private const string _redirectUrl = "https://stackexchange.com/oauth/login_success";
        //TODO: Get site from configuration
        private string _site;

        internal StackExchangeOAuthProvider(string clientId, string clientSecret, string site, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "StackExchange",
                "https://stackexchange.com/oauth/dialog",
                null,
                "https://api.stackexchange.com/2.0/me",
                clientId,
                clientSecret,
                _redirectUrl,
                scopes)
            {
                ResourceQueryParameters = new[]
                {
                    new KeyValuePair<string, string>("key", clientSecret),
                    new KeyValuePair<string, string>("site", site)
                }
            })
        {
            _site = site;
        }

        internal override string BuildGraphUrl(string token)
        {
            return string.Format(Definition.GraphUrl, token, Definition.ClientSecret, _site);
        }

        internal override AccountData GetAccountData(string json)
        {
            var jObject = JObject.Parse(json);
            var data = jObject["items"][0] as JObject;

            return new AccountData(
                data.GetStringValue("user_id"),
                data.GetStringValue("display_name"));
        }
    }
}
