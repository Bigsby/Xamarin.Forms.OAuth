using System;

namespace Xamarin.Forms.OAuth.Providers
{
    public class MeetupOAuthProvider : OAuthProvider
    {
        public MeetupOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(clientId, clientSecret, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Meetup";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://secure.meetup.com/oauth2/authorize";
            }
        }

        internal override string TokenUrl
        {
            get
            {
                return "https://secure.meetup.com/oauth2/access";
            }
        }

        protected override bool IncludeRedirectUrlInTokenRequest
        {
            get
            {
                return true;
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://api.meetup.com/2/member/self";
            }
        }

        protected override TokenType TokenType
        {
            get
            {
                return TokenType.Bearer;
            }
        }
    }
}
