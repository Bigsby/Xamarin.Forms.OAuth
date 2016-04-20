using System.Collections.Generic;
using System.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class TrelloOAuthProvider : OAuthProvider
    {
        private const string _redirectUrl = "https://trelloResponse.com";
        private string _applicationName;

        public TrelloOAuthProvider(string clientId, string clientSecret, string applicationName, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Trello",
                "https://trello.com/1/authorize",
                "https://trello.com/1/OAuthGetAccessToken",
                "https://api.trello.com/1/members/me",
                clientId,
                clientSecret,
                _redirectUrl,
                scopes)
            {
                RequiresCode = true,
                MandatoryScopes = new[] { "read" },
                TokeUrlParameter = "token",
                ResourceQueryParameters = new[] 
                {
                    new KeyValuePair<string, string>("key", clientId)
                },
                GraphNameProperty = "fullName"
            })
        {
            _applicationName = applicationName;
        }

        internal override string GetAuthorizationUrl()
        {
            var queryParameters = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("callback_method", "fragment"),
                new KeyValuePair<string, string>("return_url", _redirectUrl),
                new KeyValuePair<string, string>("expiration", "never"),
                new KeyValuePair<string, string>("name", _applicationName),
                new KeyValuePair<string, string>("key", Definition.ClientId)
            };

            var scopes = GetScopes();

            if (scopes.Any())
                queryParameters.Add(new KeyValuePair<string, string>("scope", GetScopesString(scopes)));


            return BuildUrl(Definition.AuthorizeUrl, queryParameters);
        }
    }
}
