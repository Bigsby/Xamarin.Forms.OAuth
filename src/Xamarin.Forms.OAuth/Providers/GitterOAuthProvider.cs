using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    // Does not support token refresh
    public sealed class GitterOAuthProvider : OAuthProvider
    {
        internal GitterOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Gitter",
                "https://gitter.im/login/oauth/authorize",
                "https://gitter.im/login/oauth/token",
                "https://api.gitter.im/v1/user",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                IncludeRedirectUrlInTokenRequest = true,
                TokenType = TokenType.Bearer,
                GraphNameProperty = "displayName"
            })
        { }

        internal override AccountData ReadAccountData(string json)
        {
            var jObject = JArray.Parse(json)[0];

            return base.ReadAccountData(jObject.ToString());
        }
    }
}
