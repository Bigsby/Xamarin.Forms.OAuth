using Newtonsoft.Json.Linq;
using System;

namespace Xamarin.Forms.OAuth.Providers
{
    public class InstagramOAuthProvider : OAuthProvider
    {
        internal InstagramOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(clientId, clientSecret, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Instagram";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://api.instagram.com/oauth/authorize";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.instagram.com/v1/users/self";
            }
        }

        internal override bool RequireCode
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
                return " https://api.instagram.com/oauth/access_token";
            }
        }

        internal override bool IncludeRedirectUrlInTokenRequest
        {
            get
            {
                return true;
            }
        }

        internal override Tuple<string, string> GetAccountData(string json)
        {
            var jObject = JObject.Parse(json);

            var data = jObject.GetValue("data") as JObject;

            return new Tuple<string, string>(
                data.GetStringValue("id"),
                data.GetStringValue("full_name"));
        }
    }
}
