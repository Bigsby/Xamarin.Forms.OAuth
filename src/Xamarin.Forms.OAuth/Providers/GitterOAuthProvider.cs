using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.OAuth.Providers
{
    public class GitterOAuthProvider : OAuthProvider
    {
        public GitterOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(clientId, clientSecret, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "Gitter";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://gitter.im/login/oauth/authorize";
            }
        }

        protected override bool RequiresCode
        {
            get
            {
                return true;
            }
        }

        internal override string TokenUrl
        {
            get
            {
                return "https://gitter.im/login/oauth/token";
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
                return "https://api.gitter.im/v1/user";
            }
        }

        protected override TokenType TokenType
        {
            get
            {
                return TokenType.Bearer;
            }
        }

        internal override string GraphNameProperty
        {
            get
            {
                return "displayName";
            }
        }

        internal override AccountData GetAccountData(string json)
        {
            var jObject = JArray.Parse(json)[0];

            return base.GetAccountData(jObject.ToString());
        }
    }
}
