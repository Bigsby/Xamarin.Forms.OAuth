using System.Collections.Generic;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class PayPalOAuthProvider : OAuthProvider
    {
        //TODO: Allow for sandbox definition
        internal PayPalOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "PayPal",
                "https://www.paypal.com/webapps/auth/protocol/openidconnect/v1/authorize",
                "https://www.paypal.com/webapps/auth/protocol/openidconnect/v1/tokenservice",
                //"https://api.paypal.com/v1/oauth2/token",
                "https://api.paypal.com/v1/identity/openidconnect/userinfo/",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                AuthorizationType = string.IsNullOrEmpty(clientSecret) ? AuthorizationType.Implicit : AuthorizationType.Code,
                TokenType = TokenType.Bearer,
                MandatoryScopes = new[] {"openid", "profile", "email" },
                ScopeSeparator = "%20",
                TokenAuthorizationHeaders = new[]
                {
                    BuildBasicAuthenticationHeader(clientId, clientSecret)
                },
                RefreshesToken = !string.IsNullOrEmpty(clientSecret),
                GraphIdProperty = "user_id",
                ResourceQueryParameters = new []
                {
                    new KeyValuePair<string, string>("schema", "openid")
                }
            })
        { }

        internal PayPalOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : this(clientId, null, redirectUrl, scopes)
        { }

        protected override IEnumerable<KeyValuePair<string, string>> BuildTokenRequestFields(string code)
        {
            return new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code }
            };
        }

        //protected override IEnumerable<KeyValuePair<string, string>> BuildTokenRequestFields(string code)
        //{
        //    return new Dictionary<string, string>
        //    {
        //        { "grant_type", "client_credentials" },
        //        { "assertion", code }
        //    };
        //}
    }
}
