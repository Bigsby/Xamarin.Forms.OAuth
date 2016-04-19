using Newtonsoft.Json.Linq;
using System;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class FoursquareOAuthProvider : OAuthProvider
    {
        public FoursquareOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(clientId, clientSecret, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Foursquare";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://foursquare.com/oauth2/authenticate";
            }
        }

        protected override bool RequiresCode
        {
            get
            {
                return true;
            }
        }

        internal override string TokenUrl
        {
            get
            {
                return "https://foursquare.com/oauth2/access_token";
            }
        }

        protected override bool IncludeRedirectUrlInTokenRequest
        {
            get
            {
                return true;
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.foursquare.com/v2/users/self";
            }
        }

        protected override string TokeUrlParameter
        {
            get
            {
                return "oauth_token";
            }
        }

        internal override string BuildGraphUrl(string token)
        {
            var url = base.BuildGraphUrl(token);

            return $"{url}&v=20140806";
        }

        internal override AccountData GetAccountData(string json)
        {
            var jObject = JObject.Parse(json);

            var response = jObject["response"] as JObject;
            var user = response["user"] as JObject;

            return new AccountData(user.GetStringValue("id"),
                $"{user.GetStringValue("firstName")} {user.GetStringValue("lastName")}");
        }
    }
}
