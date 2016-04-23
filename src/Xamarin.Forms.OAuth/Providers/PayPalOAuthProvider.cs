using System.Collections.Generic;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class PayPalOAuthProvider : OAuthProvider
    {
        private const string _sandboxDomain = ".sandbox";

        internal PayPalOAuthProvider(string clientId, string clientSecret, string redirectUrl, bool sandbox, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "PayPal",

                "https://www" + (sandbox ? _sandboxDomain : "") + ".paypal.com/webapps/auth/protocol/openidconnect/v1/authorize",
                "https://www" + (sandbox ? _sandboxDomain : "") + ".paypal.com/webapps/auth/protocol/openidconnect/v1/tokenservice",
                "https://api" + (sandbox ? _sandboxDomain : "") + ".paypal.com/v1/identity/openidconnect/userinfo/",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                AuthorizationType = string.IsNullOrEmpty(clientSecret) ? AuthorizationType.Implicit : AuthorizationType.Explicit,
                ExcludeClientIdAndSecretInTokenRefresh = true,

                TokenType = TokenType.Bearer,
                MandatoryScopes = new[] { "openid", "profile", "email" },
                ScopeSeparator = "%20",
                TokenAuthorizationHeaders = new[] { BuildBasicAuthenticationHeader(clientId, clientSecret) },
                RefreshTokenAuthorizationHeaders = new[] { BuildBasicAuthenticationHeader(clientId, clientSecret) },
                RefreshesToken = !string.IsNullOrEmpty(clientSecret),
                GraphIdProperty = "user_id",
                ResourceQueryParameters = new[]
                {
                    new KeyValuePair<string, string>("schema", "openid")
                }
            })
        { }

        internal PayPalOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : this(clientId, clientSecret, redirectUrl, false, scopes)
        { }
    }
}
