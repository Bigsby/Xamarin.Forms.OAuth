namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class MeetupOAuthProvider : OAuthProvider
    {
        internal MeetupOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(new OAuthProviderDefinition(
                "Meetup",
                "https://secure.meetup.com/oauth2/authorize",
                "https://secure.meetup.com/oauth2/access",
                "https://api.meetup.com/2/member/self",
                clientId,
                clientSecret,
                redirectUrl,
                scopes)
            {
                IncludeRedirectUrlInTokenRequest = true,
                TokenType = TokenType.Bearer
            })
        { }
    }
}
