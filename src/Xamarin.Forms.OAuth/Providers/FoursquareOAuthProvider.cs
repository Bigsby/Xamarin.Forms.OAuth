using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class FoursquareOAuthProvider : OAuthProvider
    {
        //TODO: get version from configuration
        private const string _version = "20140806";

        internal FoursquareOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Foursquare",
                "https://foursquare.com/oauth2/authenticate",
                "https://foursquare.com/oauth2/access_token",
                "https://api.foursquare.com/v2/users/self",
                clientId,
                clientSecret,
                redirectUrl,
                scopes
                )
            {
                RequiresCode = true,
                IncludeRedirectUrlInTokenRequest = true,
                TokenRequestUrlParameter = "oauth_token",
                ResourceQueryParameters = new[] { new KeyValuePair<string, string>("v", _version) }
            })
        { }

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
