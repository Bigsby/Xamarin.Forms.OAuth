using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xamarin.Forms.OAuth
{
    public abstract class OAuthProvider
    {
        #region Privet Fields
        private readonly string[] _scopes;
        private const string _errorParameter = "error";
        private const string _errorDescriptionParameter = "error_description";
        private const string _codeParameter = "code";
        private const string _accessTokenParatemeter = "access_token";
        private const string _refreshTokenParameter = "refresh_token";
        private const string _expiresInParameter = "expires_in";
        private static Regex _urlParameterExpression = new Regex("(.*)=(.*)");
        #endregion

        #region Constructors
        protected OAuthProvider(string clientId,
           string redirectUrl, params string[] scopes)
        {
            ClientId = clientId;
            RedirectUrl = redirectUrl;
            _scopes = scopes;
        }

        protected OAuthProvider(string clientId, string clientSecret,
            string redirectUrl, params string[] scopes)
            : this(clientId, redirectUrl, scopes)
        {
            ClientSecret = clientSecret;
        }
        #endregion

        #region Public Members
        public const string NameProperty = "Name";
        public const string LogoProperty = "Logo";
        public abstract string Name { get; }
        public virtual ImageSource Logo
        {
            get
            {
                return ImageSource.FromResource($"{GetType().Namespace}.Logos.{Name}.png", GetType().GetTypeInfo().Assembly);
            }
        }
        #endregion

        #region Internal Members
        internal virtual string GrpahIdProperty { get { return "id"; } }
        internal virtual string GraphNameProperty { get { return "name"; } }
        internal string RedirectUrl { get; private set; }
        internal virtual string TokenUrl { get { return null; } }
        internal virtual string TokenAuthorizationHeader { get { return null; } }

        internal virtual OAuthResponse GetOAuthResponseFromUrl(string url)
        {
            var parameters = ReadReponseParameter(url);

            if (parameters.ContainsKey(_errorParameter))
                return OAuthResponse.WithError(parameters[_errorParameter],
                    parameters.ContainsKey(_errorDescriptionParameter) ?
                        parameters[_errorDescriptionParameter]
                        :
                        null
                    );

            if (parameters.ContainsKey(_codeParameter))
                return OAuthResponse.WithCode(parameters[_codeParameter]);

            return OAuthResponse.WithToken(new OAuthAccessToken(
                    parameters[_accessTokenParatemeter],
                    GetExpireDate(parameters.ContainsKey(_expiresInParameter) ?
                        parameters[_expiresInParameter]
                        :
                        string.Empty)
                ));
        }

        internal virtual OAuthResponse GetTokenResponse(string response)
        {
            switch (TokenResponseSerialization)
            {
                case TokenResponseSerialization.JSON:
                    var jObject = JObject.Parse(response);

                    var error = jObject.GetStringValue(_errorParameter);
                    if (!string.IsNullOrEmpty(error))
                        return OAuthResponse.WithError(error, jObject.GetStringValue(_errorDescriptionParameter));

                    return OAuthResponse.WithToken(new OAuthAccessToken(
                        jObject.GetStringValue(_accessTokenParatemeter),
                        jObject.GetStringValue(_refreshTokenParameter),
                        GetExpireDate(jObject.GetStringValue(_expiresInParameter))));
                case TokenResponseSerialization.Forms:
                    return GetOAuthResponseFromUrl("http://abc.com?" + response);
                default:
                    return OAuthResponse.WithError("TokenResponseError", "Unknow serialization.");
            }
        }

        internal virtual AccountData GetAccountData(string json)
        {
            var jObject = JObject.Parse(json);
            return new AccountData(
                jObject.GetStringValue(GrpahIdProperty),
                jObject.GetStringValue(GraphNameProperty));
        }

        internal virtual string GetAuthorizationUrl()
        {
            var scopesToInject = MandatoryScopes.Union(_scopes ?? new string[0]).Distinct().ToArray();
            var scope = scopesToInject.Any() ?
                "&scope=" + string.Join(ScopeSeparator, scopesToInject)
                :
                string.Empty;

            var state = IncludeStateInAuthorize ?
                "&state=authentication"
                :
                string.Empty;

            return $"{AuthorizeUrl}?response_type={AuthorizeResponseType}&client_id={ClientId}&redirect_uri={WebUtility.UrlEncode(RedirectUrl)}{scope}{state}";
        }

        internal virtual IEnumerable<KeyValuePair<string, string>> BuildTokenRequestHeaders()
        {
            return new KeyValuePair<string, string>[0];
        }

        internal virtual string BuildTokenContent(string code)
        {
            var fields = BuildTokenRequestFields(code);

            return string.Join("&", fields.Select(pair => $"{pair.Key}={pair.Value}"));
        }

        protected virtual IEnumerable<KeyValuePair<string, string>> BuildTokenRequestFields(string code)
        {
            var fields = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code }
            };

            if (IncludeRedirectUrlInTokenRequest)
                fields.Add("redirect_uri", WebUtility.UrlEncode(RedirectUrl));

            if (!ExcludeClientIdInTokenRequest)
                fields.Add("client_id", ClientId);

            if (!string.IsNullOrEmpty(ClientSecret))
                fields.Add("client_secret", ClientSecret);

            return fields;
        }

        internal virtual string BuildGraphUrl(string token)
        {
            return $"{GraphUrl}?access_token={token}";
        }

        internal virtual IEnumerable<KeyValuePair<string, string>> GraphHeaders(OAuthAccessToken token)
        {
            return TokenType == TokenType.Url ?
                new KeyValuePair<string, string>[0]
                :
                new[] { new KeyValuePair<string, string>("Authorization", $"Bearer {token.Token}") };
        }

        internal virtual async Task PreAuthenticationProcess() { await Task.FromResult(0); }

        public class AccountData
        {
            public AccountData(string id, string name)
            {
                Id = id;
                Name = name;
            }
            public string Id { get; private set; }
            public string Name { get; private set; }
        }
        #endregion

        #region Protected Members
        protected string ClientId { get; private set; }
        protected string ClientSecret { get; private set; }
        protected virtual string[] MandatoryScopes { get { return new string[0]; } }
        protected abstract string AuthorizeUrl { get; }
        protected abstract string GraphUrl { get; }
        protected virtual bool RequiresCode { get { return false; } }
        protected virtual bool ExcludeClientIdInTokenRequest { get { return false; } }
        //protected virtual bool IsTokenResponseJson { get { return true; } }
        protected virtual bool IncludeRedirectUrlInTokenRequest { get { return false; } }
        protected virtual bool IncludeStateInAuthorize { get { return false; } }
        protected virtual string ScopeSeparator { get { return ","; } }
        protected virtual TokenType TokenType { get { return TokenType.Url; } }
        protected virtual TokenResponseSerialization TokenResponseSerialization { get { return TokenResponseSerialization.JSON; } }

        protected virtual string AuthorizeResponseType
        {
            get { return RequiresCode ? "code" : "token"; }
        }

        protected static IDictionary<string, string> ReadReponseParameter(string url)
        {
            var uri = new Uri(url);
            var query = uri.Query.Trim('?');

            if (string.IsNullOrEmpty(query))
                query = uri.Fragment.Trim('#');

            var parameters = query.Split('&');

            var result = new Dictionary<string, string>();

            foreach (var parameter in parameters)
            {
                var match = _urlParameterExpression.Match(parameter);
                if (match.Success)
                    result.Add(match.Groups[1].Value, match.Groups[2].Value);
            }

            return result;
        }

        protected static DateTime GetExpireDate(string value)
        {
            return string.IsNullOrEmpty(value) ?
                DateTime.MinValue
                :
                DateTime.Now + TimeSpan.FromSeconds(double.Parse(value));
        }
        #endregion
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
