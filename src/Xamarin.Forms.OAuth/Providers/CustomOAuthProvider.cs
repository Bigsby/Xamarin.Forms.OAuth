using System;
using System.Collections.Generic;

namespace Xamarin.Forms.OAuth.Providers
{
    public sealed class CustomOAuthProvider : OAuthProvider
    {
        internal CustomOAuthProvider(OAuthProviderDefinition definition)
            : base(definition)
        { }

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
        internal override AccountData ReadAccountData(string json)
        {
            return null == _accountDataGetter ? base.ReadAccountData(json) : _accountDataGetter(json);
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
        internal override OAuthResponse ReadOAuthResponseFromUrl(string url)
        {
            return null == _oAuthResponseFromUrlGetter ? base.ReadOAuthResponseFromUrl(url) : _oAuthResponseFromUrlGetter(url);
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
        internal override IEnumerable<KeyValuePair<string, string>> ResourceHeaders(OAuthAccessToken token)
        {
            return null == _graphHeadersBuilder ? base.ResourceHeaders(token) : _graphHeadersBuilder(token);
        }
        #endregion
    }
}
