using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    public class LinkedInOAuthProvider : OAuthProvider
    {
        public LinkedInOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(clientId, clientSecret, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "LinkedIn";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://www.linkedin.com/uas/oauth2/authorization";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.linkedin.com/v1/people/~";
            }
        }

        protected override string TokeUrlParameter
        {
            get
            {
                return "oauth2_access_token";
            }
        }
        protected override IEnumerable<KeyValuePair<string, string>> ResourceQueryParameters
        {
            get
            {
                return new[] { new KeyValuePair<string, string>("format", "json") };
            }
        }

        protected override bool IncludeStateInAuthorize
        {
            get
            {
                return true;
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
                return "https://www.linkedin.com/uas/oauth2/accessToken";
            }
        }

        protected override bool IncludeRedirectUrlInTokenRequest
        {
            get
            {
                return true;
            }
        }

        internal override AccountData GetAccountData(string json)
        {
            var jObject = JObject.Parse(json);

            return new AccountData(
                jObject.GetStringValue("id"),
                $"{jObject.GetStringValue("firstName")} {jObject.GetStringValue("lastName")}");
        }
    }
}
