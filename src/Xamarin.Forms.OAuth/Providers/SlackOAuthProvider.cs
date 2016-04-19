using System;

namespace Xamarin.Forms.OAuth.Providers
{
    public class SlackOAuthProvider : OAuthProvider
    {
        public SlackOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(clientId, clientSecret, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Slack";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://slack.com/oauth/authorize";
            }
        }

        protected override bool RequiresCode
        {
            get
            {
                return true;            }
        }

        internal override string TokenUrl
        {
            get
            {
                return "https://slack.com/api/oauth.access";
            }
        }

        protected override string[] MandatoryScopes
        {
            get
            {
                return new[] { "identify" };
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://slack.com/api/auth.test?token={0}";
            }
        }

        internal override string GrpahIdProperty
        {
            get
            {
                return "user_id";
            }
        }

        internal override string GraphNameProperty
        {
            get
            {
                return "user";
            }
        }

        internal override string BuildGraphUrl(string token)
        {
            return string.Format(GraphUrl, token);
        }
    }
}
