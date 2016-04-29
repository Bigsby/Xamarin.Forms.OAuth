using Newtonsoft.Json.Linq;

namespace Bigs.OAuth.Providers
{
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
                RequiresCode = true,
                IncludeRedirectUrlInTokenRequest = true,
                TokenType = TokenType.Bearer,
                GraphNameProperty = "displayName"
            })
        { }

        internal override AccountData GetAccountData(string json)
        {
            var jObject = JArray.Parse(json)[0];

            return base.GetAccountData(jObject.ToString());
        }
    }
}
