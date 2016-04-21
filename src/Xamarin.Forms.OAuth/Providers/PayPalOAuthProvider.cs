using System.Collections.Generic;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class PayPalOAuthProvider : OAuthProvider
    {
        //TODO: Allow for sandbox definition
        public PayPalOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "PayPal",
                "https://www.paypal.com/webapps/auth/protocol/openidconnect/v1/authorize",
                "https://api.paypal.com/v1/oauth2/token",
                "https://api.paypal.com/v1/identity/openidconnect/userinfo/",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                RequiresCode = true,
                TokenType = TokenType.Bearer,
                MandatoryScopes = new[] { "profile" },
                TokenAuthorizationHeaders = new[]
                {
                    BuildBasicAuthenticationHeader(clientId, clientSecret)
                },
                GraphIdProperty = "email",
                ResourceQueryParameters = new []
                {
                    new KeyValuePair<string, string>("schema", "openid")
                }
            })
        { }
    }
}
