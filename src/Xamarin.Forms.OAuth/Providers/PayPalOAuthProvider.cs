namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class PayPalOAuthProvider : OAuthProvider
    {
        public PayPalOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "https://www.paypal.com/webapps/auth/protocol/openidconnect/v1/authorize",
                "https://api.paypal.com/v1/oauth2/token",
                "",
                "",
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
                }
            })
        { }
    }
}
