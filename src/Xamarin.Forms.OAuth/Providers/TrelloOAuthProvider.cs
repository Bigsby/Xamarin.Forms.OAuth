using System.Collections.Generic;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class TrelloOAuthProvider : OAuthProvider
    {
        private const string _redirectUrl = "https://trelloResponse.com";
        public TrelloOAuthProvider(string clientId, string clientSecret, params string[] scopes)
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
            })
        { }

        internal override string GetAuthorizationUrl()
        {
            return BuildUrl(Definition.AuthorizeUrl, new KeyValuePair<string, string>[] { });
        }
    }
}
