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
            string graphUrl,
            string clientId,
            string clientSecret,
            string redirectUrl, 
            params string[] scopes)
        {
            Name = name;
            AuthorizeUrl = authorizeUrl;
            TokenUrl = tokenUrl;
            GraphUrl = graphUrl;
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUrl = redirectUrl;
            Scopes = scopes ?? new string[0];
        }

        internal string Name { get; private set; }
        internal string AuthorizeUrl { get; private set; }
        internal string TokenUrl { get; private set; }
        
        internal string GraphUrl { get; private set; }
        internal string ClientId { get; private set; }
        internal string ClientSecret { get; private set; }
        internal string RedirectUrl { get; private set; }
        internal string[] Scopes { get; private set; }

        
        public AuthorizationType AuthorizationType {internal get; set; } = AuthorizationType.Explicit;
        public IEnumerable<KeyValuePair<string, string>> AuthorizeCustomQueryParameters { internal get; set; } = new KeyValuePair<string, string>[0];
        public bool RefreshesToken { internal get; set; } = false;
        public bool ExcludeClientIdInTokenRequest { internal get; set; } = false;
        public bool ExcludeClientSecretInTokenRequest { internal get; set; } = false;
        public bool ExcludeClientIdAndSecretInTokenRefresh { internal get; set; } = false;
        public bool IncludeRedirectUrlInTokenRequest { internal get; set; } = false;
        public bool IncludeRedirectUrlInRefreshTokenRequest { internal get; set; } = false;
        public bool IncludeStateInAuthorize { internal get; set; } = false;
        public string ScopeSeparator { internal get; set; } = ",";
        public TokenType TokenType { internal get; set; } = TokenType.Url;
        public string BearerTokenType { internal get; set; } = "Bearer";
        public TokenResponseSerialization TokenResponseSerialization { internal get; set; } = TokenResponseSerialization.JSON;
        public string ExpiresParameter { internal get; set; } = "expires_in";
        //public bool RequiresCode { internal get; set; } = false;
        public string[] MandatoryScopes { internal get; set; } = new string[0];
        public string TokenResponseUrlParameter { internal get; set; } = "access_token";
        public string TokenRequestUrlParameter { internal get; set; } = "access_token";
        public IEnumerable<KeyValuePair<string, string>> ResourceQueryParameters { internal get; set; } = new KeyValuePair<string, string>[0];
        public ImageSource Logo { internal get; set; } = null;
        public string GraphIdProperty { internal get; set; } = "id";
        public string GraphNameProperty { internal get; set; } = "name";
        public IEnumerable<KeyValuePair<string, string>> TokenAuthorizationHeaders { internal get; set; } = new KeyValuePair<string, string>[0];
        public IEnumerable<KeyValuePair<string, string>> RefreshTokenAuthorizationHeaders { internal get; set; } = new KeyValuePair<string, string>[0];

        public string AuthorizeResponseType
        {
            internal get
            {
                return string.IsNullOrEmpty(_authorizeResponseType) ?
                     AuthorizationType == AuthorizationType.Explicit ? "code" : "token"
                     :
                     _authorizeResponseType;
            }
            set { _authorizeResponseType = value; }
        }
    }

    public enum TokenType
    {
        Unknown,
        Url,
        Bearer
    }

    public enum TokenResponseSerialization
    {
        JSON,
        UrlEncoded
    }

    public enum AuthorizationType
    {
        Implicit,
        Explicit
    }
}