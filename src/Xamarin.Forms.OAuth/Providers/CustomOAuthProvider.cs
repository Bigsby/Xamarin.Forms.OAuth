using System;
using System.Collections.Generic;

namespace Xamarin.Forms.OAuth.Providers
{
    public class CustomOAuthProvider : OAuthProvider
    {
        private readonly string _name;
        private readonly string _authoizationUrl;
        private readonly string _redirectUrl;
        private readonly string _graphUrl;
        private readonly string _idProperty;
        private readonly string _nameProperty;

        private ImageSource _logo;

        internal CustomOAuthProvider(
            string name,
            string authorizationUrl,
            string redirectUrl,
            string graphUrl,
            string clientId,
            string idProperty = null,
            string nameProperty = null,
            ImageSource logo = null,
            params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        {
            _name = name;
            _authoizationUrl = authorizationUrl;
            _redirectUrl = redirectUrl;
            _graphUrl = graphUrl;
            _idProperty = idProperty;
            _nameProperty = nameProperty;
            _logo = logo;
            _excludeClientIdInTokenRequest = base.ExcludeClientIdInTokenRequest;
            _includeRedirectUrlInTokenRequest = base.IncludeRedirectUrlInTokenRequest;
            _includeStateInAuthorize = base.IncludeStateInAuthorize;
            _isTokenResponseJson = base.IsTokenResponseJson;
            _requiresCode = base.RequiresCode;
        }

        #region Constructor Members
        public override string Name
        {
            get
            {
                return _name;
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return _authoizationUrl;
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return _graphUrl;
            }
        }

        public override ImageSource Logo
        {
            get
            {
                return _logo;
            }
        }

        internal override string GrpahIdProperty
        {
            get
            {
                return string.IsNullOrEmpty(_idProperty) ? base.GrpahIdProperty : _idProperty;
            }
        }

        internal override string GraphNameProperty
        {
            get
            {
                return string.IsNullOrEmpty(_nameProperty) ? base.GraphNameProperty : _nameProperty;
            }
        }
        #endregion

        #region Property Overriding
        private bool _excludeClientIdInTokenRequest;
        public void SetExcludeClientIdInTokenRequest(bool value)
        {
            _excludeClientIdInTokenRequest = value;
        }
        protected override bool ExcludeClientIdInTokenRequest
        {
            get
            {
                return _excludeClientIdInTokenRequest;
            }
        }

        private bool _includeRedirectUrlInTokenRequest;
        public void SetIncludeRedirectUrlInTokenRequest(bool value)
        {
            _includeRedirectUrlInTokenRequest = value;
        }
        protected override bool IncludeRedirectUrlInTokenRequest
        {
            get
            {
                return _includeRedirectUrlInTokenRequest;
            }
        }

        private bool _includeStateInAuthorize;
        public void SetIncludeStateInAuthorize(bool value)
        {
            _includeStateInAuthorize = value;
        }
        protected override bool IncludeStateInAuthorize
        {
            get
            {
                return _includeStateInAuthorize;
            }
        }

        private bool _isTokenResponseJson;
        public void SetIsTokenResponseJson(bool value)
        {
            _isTokenResponseJson = value;
        }
        protected override bool IsTokenResponseJson
        {
            get
            {
                return _isTokenResponseJson;
            }
        }

        private bool _requiresCode;
        public void SetRequiresCode(bool value)
        {
            _requiresCode = value;
        }
        protected override bool RequiresCode
        {
            get
            {
                return _requiresCode;
            }
        }

        private string _tokenAuthorizationHeader;
        public void SetTokenAuthorizationHeader(string value)
        {
            _tokenAuthorizationHeader = value;
        }
        internal override string TokenAuthorizationHeader
        {
            get
            {
                return _tokenAuthorizationHeader;
            }
        }

        private string _tokenUrl;
        public void SetTokenUrl(string url)
        {
            _tokenUrl = url;
        }
        internal override string TokenUrl
        {
            get
            {
                return _tokenUrl;
            }
        }
        #endregion

        #region Method Overriding
        private Func<string, string> _graphUrlBuilder;
        public void SetGraphUrlBuilder(Func<string, string> graphUrlBuilder)
        {
            _graphUrlBuilder = graphUrlBuilder;
        }
        internal override string BuildGraphUrl(string token)
        {
            return null == _graphUrlBuilder ? base.BuildGraphUrl(token) : _graphUrlBuilder(token);
        }

        private Func<string, AccountData> _accountDataGetter;
        public void SetAccountDataGetter(Func<string, AccountData> getter)
        {
            _accountDataGetter = getter;
        }
        internal override AccountData GetAccountData(string json)
        {
            return null == _accountDataGetter ? base.GetAccountData(json) : _accountDataGetter(json);
        }

        private Func<string> _authorizationUrlGetter;
        public void SetAuthorizationUrlGetter(Func<string> getter)
        {
            _authorizationUrlGetter = getter;
        }
        internal override string GetAuthorizationUrl()
        {
            return base.GetAuthorizationUrl();
        }

        private Func<string, OAuthResponse> _oAuthResponseFromUrlGetter;
        public void SetOAuthResponseFromUrlGetter(Func<string, OAuthResponse> getter)
        {
            _oAuthResponseFromUrlGetter = getter;
        }
        internal override OAuthResponse GetOAuthResponseFromUrl(string url)
        {
            return null == _oAuthResponseFromUrlGetter ? base.GetOAuthResponseFromUrl(url) : _oAuthResponseFromUrlGetter(url);
        }

        private Func<string, OAuthResponse> _tokenResponseGetter;
        public void SetTokenResponseGetter(Func<string, OAuthResponse> getter)
        {
            _tokenResponseGetter = getter;
        }
        internal override OAuthResponse GetTokenResponse(string response)
        {
            return null == _tokenResponseGetter ? base.GetTokenResponse(response) : _tokenResponseGetter(response);
        }

        private Func<OAuthAccessToken, IEnumerable<KeyValuePair<string, string>>> _graphHeadersBuilder;
        public void SetGraphHeadersBuilder(Func<OAuthAccessToken, IEnumerable<KeyValuePair<string, string>>> builder)
        {
            _graphHeadersBuilder = builder;
        }
        internal override IEnumerable<KeyValuePair<string, string>> GraphHeaders(OAuthAccessToken token)
        {
            return null == _graphHeadersBuilder ? base.GraphHeaders(token) : _graphHeadersBuilder(token);
        }
        #endregion
    }
}
