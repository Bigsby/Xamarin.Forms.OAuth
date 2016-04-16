using Newtonsoft.Json.Linq;
using System;

namespace Xamarin.Forms.OAuth.Providers
{
    public class StackExchangeOAuthProvider : OAuthProvider
    {
        private const string _redirectUrl = "https://stackexchange.com/oauth/login_success";
        //TODO: Get site from configuration
        private const string _site = "stackoverflow";

        public StackExchangeOAuthProvider(string clientId, string clientSecret, params string[] scopes)
            : base(clientId, clientSecret, _redirectUrl, scopes)
        { }

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
                return "https://api.stackexchange.com/2.0/me?access_token={0}&key={1}&site={2}";
            }
        }

        internal override string BuildGraphUrl(string token)
        {
            return string.Format(GraphUrl, token, ClientSecret, _site);
        }

        internal override Tuple<string, string> GetAccountData(string json)
        {
            var jObject = JObject.Parse(json);
            var data = jObject["items"][0] as JObject;

            return new Tuple<string, string>(
                data.GetStringValue("user_id"),
                data.GetStringValue("display_name")
                );
        }
    }
}
