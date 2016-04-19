using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    public class StackExchangeOAuthProvider : OAuthProvider
    {
        private const string _redirectUrl = "https://stackexchange.com/oauth/login_success";
        //TODO: Get site from configuration
        private string _site;

        public StackExchangeOAuthProvider(string clientId, string clientSecret, string site, params string[] scopes)
            : base(clientId, clientSecret, _redirectUrl, scopes)
        {
            _site = site;
        }

        public override string Name
        {
            get
            {
                return "StackExchange";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://stackexchange.com/oauth/dialog";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.stackexchange.com/2.0/me";
            }
        }

        protected override IEnumerable<KeyValuePair<string, string>> ResourceQueryParameters
        {
            get
            {
                return new[] 
                {
                    new KeyValuePair<string, string>("key", ClientSecret),
                    new KeyValuePair<string, string>("site", _site)
                };
            }
        }

        internal override string BuildGraphUrl(string token)
        {
            return string.Format(GraphUrl, token, ClientSecret, _site);
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
