using System.Collections.Generic;

namespace Xamarin.Forms.OAuth
{
    public sealed class OAuthProviderDefinition
    {
        private string _authorizeResponseType = null;

        public OAuthProviderDefinition(
            string name,
            string authorizeUrl,
            string tokenUrl,
            string refreshTokenUrl,
            string graphUrl,
            string clientId,
            string clientSecret,
            string redirectUrl, 
            params string[] scopes)
        {
            Name = name;
            AuthorizeUrl = authorizeUrl;
            TokenUrl = tokenUrl;
            RefreshTokenUrl = refreshTokenUrl;
            GraphUrl = graphUrl;
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUrl = redirectUrl;
            Scopes = scopes ?? new string[0];
        }

        internal string Name { get; private set; }
        internal string AuthorizeUrl { get; private set; }
        internal string TokenUrl { get; private set; }
        internal string RefreshTokenUrl { get; private set; }
        internal string GraphUrl { get; private set; }
        internal string ClientId { get; private set; }
        internal string ClientSecret { get; private set; }
        internal string RedirectUrl { get; private set; }
        internal string[] Scopes { get; private set; }

        public AuthorizationType AuthorizationType {internal get; set; } = AuthorizationType.Code;
        public bool ExcludeClientIdInTokenRequest { internal get; set; } = false;
        public bool IncludeRedirectUrlInTokenRequest { internal get; set; } = false;
        public bool IncludeStateInAuthorize { internal get; set; } = false;
        public string ScopeSeparator { internal get; set; } = ",";
        public TokenType TokenType { internal get; set; } = TokenType.Url;
        public TokenResponseSerialization TokenResponseSerialization { internal get; set; } = TokenResponseSerialization.JSON;
        //public bool RequiresCode { internal get; set; } = false;
        public string[] MandatoryScopes { internal get; set; } = new string[0];
        public string TokenResponseUrlParameter { internal get; set; } = "access_token";
        public string TokenRequestUrlParameter { internal get; set; } = "access_token";
        public IEnumerable<KeyValuePair<string, string>> ResourceQueryParameters { internal get; set; } = new KeyValuePair<string, string>[0];
        public ImageSource Logo { internal get; set; } = null;
        public string GraphIdProperty { internal get; set; } = "id";
        public string GraphNameProperty { internal get; set; } = "name";
        public IEnumerable<KeyValuePair<string, string>> TokenAuthorizationHeaders { internal get; set; } = new KeyValuePair<string, string>[0];

        public string AuthorizeResponseType
        {
            internal get
            {
                return string.IsNullOrEmpty(_authorizeResponseType) ?
                     AuthorizationType == AuthorizationType.Code ? "code" : "token"
                     :
                     _authorizeResponseType;
            }
            set { _authorizeResponseType = value; }
        }
    }

    public enum TokenType
    {
        Url,
        Bearer
    }

    public enum TokenResponseSerialization
    {
        JSON,
        Forms
    }

    public enum AuthorizationType
    {
        Token,
        Code
    }
}