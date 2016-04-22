using System.Collections.Generic;
using System.Net;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class VisualStudioOAuthProvider : OAuthProvider
    {
        // https://www.visualstudio.com/integrate/get-started/auth/oauth
        public VisualStudioOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "VisualStudio",
                "https://app.vssps.visualstudio.com/oauth2/authorize",
                "https://app.vssps.visualstudio.com/oauth2/token",
                "https://app.vssps.visualstudio.com/_apis/profile/profiles/me",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                ScopeSeparator = " ",
                AuthorizeResponseType = "Assertion",
                IncludeStateInAuthorize = true,
                RefreshesToken = true,
                ResourceQueryParameters = new[] { new KeyValuePair<string, string>("api-version", "1.0") },
                GraphNameProperty = "displayName",
                TokenType = TokenType.Bearer
            })
        { }

        protected override IEnumerable<KeyValuePair<string, string>> BuildTokenRequestFields(string code)
        {
            return BuildRequestFiles("urn:ietf:params:oauth:grant-type:jwt-bearer", code);
        }

        protected override IEnumerable<KeyValuePair<string, string>> BuildRefreshTokenRequestFields(string code)
        {
            return BuildRequestFiles("refresh_token", code);
        }

        private IEnumerable<KeyValuePair<string, string>> BuildRequestFiles(string grantType, string code)
        {
            return new Dictionary<string, string>
            {
                { "grant_type", grantType },
                { "assertion", code },
                { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                { "redirect_uri", WebUtility.UrlEncode(Definition.RedirectUrl) },
                { "client_assertion", Definition.ClientSecret }
            };
        }
    }
}
