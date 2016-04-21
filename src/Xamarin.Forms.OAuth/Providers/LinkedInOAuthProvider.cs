using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class LinkedInOAuthProvider : OAuthProvider
    {
        internal LinkedInOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "LinkedIn",
                "https://www.linkedin.com/uas/oauth2/authorization",
                "https://www.linkedin.com/uas/oauth2/accessToken",
                "https://api.linkedin.com/v1/people/~",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                TokenRequestUrlParameter = "oauth2_access_token",
                ResourceQueryParameters = new[] { new KeyValuePair<string, string>("format", "json") },
                IncludeStateInAuthorize = true,
                RequiresCode = true,
                IncludeRedirectUrlInTokenRequest = true
            })
        { }

        internal override AccountData GetAccountData(string json)
        {
            var jObject = JObject.Parse(json);

            return new AccountData(
                jObject.GetStringValue("id"),
                $"{jObject.GetStringValue("firstName")} {jObject.GetStringValue("lastName")}");
        }
    }
}
