﻿namespace Xamarin.Forms.OAuth.Providers
{
    // Does not support token refresh
    public sealed class SlackOAuthProvider : OAuthProvider
    {
        internal SlackOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Slack",
                "https://slack.com/oauth/authorize",
                "https://slack.com/api/oauth.access",
                "https://slack.com/api/auth.test",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                MandatoryScopes = new[] { "identify" },
                TokenRequestUrlParameter = "token",
                GraphIdProperty = "user_id",
                GraphNameProperty = "user"
            })
        { }
    }
}
