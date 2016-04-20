using System.Collections.Generic;

namespace Xamarin.Forms.OAuth
{
    public sealed class OAuthProviderDefinition
    {
        private string[] _mandatoryScopes = new string[0];
        private bool _requiresCode = false;
        private bool _excludeClientIdInTokenRequest = false;
        private bool _includeRedirectUrlInTokenRequest = false;
        private bool _includeStateInAuthorize = false;
        private string _scopeSeparator = ",";
        private TokenType _tokenType = TokenType.Url;
        private TokenResponseSerialization _tokenResponseSerialization = TokenResponseSerialization.JSON;
        private string _tokeUrlParameter = "access_token";
        private string _authorizeResponseType = null;
        private IEnumerable<KeyValuePair<string, string>> _resourceQueryParameters = new KeyValuePair<string, string>[0];
        private ImageSource _logo;
        private string _graphIdProperty = "id";
        private string _graphNameProperty = "name";
        //private string _tokenAuthorizationHeader = null;
        private IEnumerable<KeyValuePair<string, string>> _tokenAuthorizationHeaders = new KeyValuePair<string, string>[0];
        private string[] _scopes;

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
            _scopes = scopes;
        }

        internal string Name { get; private set; }
        internal string AuthorizeUrl { get; private set; }
        internal string TokenUrl { get; private set; }
        internal string ClientId { get; private set; }
        internal string ClientSecret { get; private set; }
        internal string RedirectUrl { get; private set; }
        internal string GraphUrl { get; private set; }

        public bool ExcludeClientIdInTokenRequest
        {
            internal get { return _excludeClientIdInTokenRequest; }
            set { _excludeClientIdInTokenRequest = value; }
        }

        public bool IncludeRedirectUrlInTokenRequest
        {
            internal get { return _includeRedirectUrlInTokenRequest; }
            set { _includeRedirectUrlInTokenRequest = value; }
        }

        public bool IncludeStateInAuthorize
        {
            internal get { return _includeStateInAuthorize; }
            set { _includeStateInAuthorize = value; }
        }

        public string ScopeSeparator
        {
            internal get { return _scopeSeparator; }
            set { _scopeSeparator = value; }
        }

        public TokenType TokenType
        {
            internal get { return _tokenType; }
            set { _tokenType = value; }
        }

        public TokenResponseSerialization TokenResponseSerialization
        {
            internal get { return _tokenResponseSerialization; }
            set { _tokenResponseSerialization = value; }
        }

        public bool RequiresCode
        {
            internal get { return _requiresCode; }
            set { _requiresCode = value; }
        }

        public string[] MandatoryScopes
        {
            internal get { return _mandatoryScopes; }
            set { _mandatoryScopes = value; }
        }

        public string TokeUrlParameter
        {
            internal get { return _tokeUrlParameter; }
            set { _tokeUrlParameter = value; }
        }

        public string AuthorizeResponseType
        {
            internal get
            {
                return string.IsNullOrEmpty(_authorizeResponseType) ?
                     RequiresCode ? "code" : "token"
                     :
                     _authorizeResponseType;
            }
            set { _authorizeResponseType = value; }
        }

        public IEnumerable<KeyValuePair<string, string>> ResourceQueryParameters
        {
            internal get { return _resourceQueryParameters; }
            set { _resourceQueryParameters = value; }
        }

        public ImageSource Logo
        {
            internal get { return _logo; }
            set { _logo = value; }
        }

        public string GraphIdProperty
        {
            internal get { return _graphIdProperty; }
            set { _graphIdProperty = value; }
        }

        public string GraphNameProperty
        {
            internal get { return _graphNameProperty; }
            set { _graphNameProperty = value; }
        }

        internal string[] Scopes
        {
            get { return _scopes; }
        }

        //public string TokenAuthorizationHeader
        //{
        //    internal get { return _tokenAuthorizationHeader; }
        //    set { _tokenAuthorizationHeader = value; }
        //}

        public IEnumerable<KeyValuePair<string, string>> TokenAuthorizationHeaders
        {
            internal get { return _tokenAuthorizationHeaders; }
            set { _tokenAuthorizationHeaders = value; }
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
}
