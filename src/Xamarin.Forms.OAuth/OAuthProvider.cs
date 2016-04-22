using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xamarin.Forms.OAuth
{
    public abstract class OAuthProvider
    {
        #region Privet Fields
        private const string _errorParameter = "error";
        private const string _errorDescriptionParameter = "error_description";
        private const string _codeParameter = "code";
        private const string _accessTokenParatemeter = "access_token";
        private const string _tokenTypeParameter = "token_type";
        private const string _refreshTokenParameter = "refresh_token";
        private const string _expiresInParameter = "expires_in";
        private static Regex _urlParameterExpression = new Regex("(.*)=(.*)");
        private readonly OAuthProviderDefinition _definition;
        #endregion

        #region Constructor
        protected OAuthProvider(OAuthProviderDefinition definition)
        {
            _definition = definition;
        }
        #endregion

        #region Public Members
        public const string NameProperty = "Name";
        public const string LogoProperty = "Logo";
        public string Name { get { return Definition.Name; } }
        public ImageSource Logo
        {
            get
            {
                return Definition.Logo ?? ImageSource.FromResource($"{GetType().Namespace}.Logos.{Name}.png", GetType().GetTypeInfo().Assembly);
            }
        }


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

        #region Internal Members
        internal virtual OAuthResponse ReadOAuthResponseFromUrl(string url)
        {
            var parameters = ReadResponseParameter(url);

            if (parameters.ContainsKey(_errorParameter))
                return OAuthResponse.WithError(parameters[_errorParameter],
                    parameters.ContainsKey(_errorDescriptionParameter) ?
                        parameters[_errorDescriptionParameter]
                        :
                        null
                    );

            if (parameters.ContainsKey(_codeParameter))
                return OAuthResponse.WithCode(parameters[_codeParameter]);

            var tokenTypeValue = parameters.ContainsKey(_tokenTypeParameter)
                ?
                parameters[_tokenTypeParameter]
                : string.Empty;

            return OAuthResponse.WithToken(new OAuthAccessToken(
                    parameters[Definition.TokenResponseUrlParameter],
                    "bearer".Equals(tokenTypeValue, StringComparison.OrdinalIgnoreCase) ?
                    TokenType.Bearer
                    :
                    TokenType.Url,
                    GetExpireDate(parameters.ContainsKey(_expiresInParameter) ?
                        parameters[_expiresInParameter]
                        :
                        string.Empty)
                ));
        }

        internal virtual async Task<T> GetResource<T>(string resourceUrl, OAuthAccessToken token, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
            where T : class
        {
            var url = BuildResourceTokenUrl(resourceUrl, token, queryParameters);

            using (var client = new HttpClient())
            {
                var graphHeaders = ResourceHeaders(token);

                foreach (var header in graphHeaders)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);

                var response = await client.GetAsync(url);

                return await ReadHttpResponse<T>(response);
            }
        }

        internal virtual async Task<T> PostToResource<T>(string resourceUrl, HttpContent content, OAuthAccessToken token, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
            where T : class
        {
            var url = BuildResourceTokenUrl(resourceUrl, token, queryParameters);

            using (var client = new HttpClient())
            {
                var graphHeaders = CustomResourceHeaders(token);

                foreach (var header in graphHeaders)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);

                var response = await client.PostAsync(url, content);

                return await ReadHttpResponse<T>(response);
            }
        }

        internal virtual OAuthResponse GetTokenResponse(string response)
        {
            switch (Definition.TokenResponseSerialization)
            {
                case TokenResponseSerialization.JSON:
                    return GetOAuthResponseFromJson(response);
                case TokenResponseSerialization.UrlEncoded:
                    return ReadOAuthResponseFromUrl("http://abc.com?" + response);
                default:
                    return OAuthResponse.WithError("TokenResponseError", "Unknow serialization.");
            }
        }

        internal virtual AccountData ReadAccountData(string json)
        {
            var jObject = JObject.Parse(json);
            return new AccountData(
                jObject.GetStringValue(Definition.GraphIdProperty),
                jObject.GetStringValue(Definition.GraphNameProperty));
        }

        internal virtual string GetAuthorizationUrl()
        {
            var queryParameters = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("response_type", Definition.AuthorizeResponseType),
                new KeyValuePair<string, string>("client_id", Definition.ClientId),
                new KeyValuePair<string, string>("redirect_uri", WebUtility.UrlEncode(Definition.RedirectUrl))
            };

            var scopesToInject = GetScopes();
            if (scopesToInject.Any())
                queryParameters.Add(new KeyValuePair<string, string>("scope", GetScopesString(scopesToInject)));

            if (Definition.IncludeStateInAuthorize)
                queryParameters.Add(new KeyValuePair<string, string>("state", "authorization"));

            queryParameters.AddRange(Definition.AuthorizeCustomQueryParameters);

            return BuildUrl(Definition.AuthorizeUrl, queryParameters);
        }

        internal virtual IEnumerable<KeyValuePair<string, string>> BuildTokenRequestHeaders()
        {
            return new KeyValuePair<string, string>[0];
        }

        internal virtual IEnumerable<KeyValuePair<string, string>> CustomResourceHeaders(OAuthAccessToken token)
        {
            return new KeyValuePair<string, string>[0];
        }

        internal virtual IEnumerable<KeyValuePair<string, string>> ResourceHeaders(OAuthAccessToken token)
        {
            return CustomResourceHeaders(token).Union(
                Definition.TokenType == TokenType.Url ?
                new KeyValuePair<string, string>[0]
                :
                new[] { new KeyValuePair<string, string>("Authorization", $"{Definition.BearerTokenType} {token.Token}") });
        }

        internal virtual async Task PreAuthenticationProcess() { await Task.FromResult(0); }

        internal static async Task<string> ReadContent(HttpContent content)
        {
            if (content.Headers.ContentEncoding.Contains("gzip"))
                return await new StreamReader(new Ionic.Zlib.GZipStream(await content.ReadAsStreamAsync(),
                    Ionic.Zlib.CompressionMode.Decompress
                    )).ReadToEndAsync();

            return await content.ReadAsStringAsync();
        }

        internal bool CheckRedirect(string url)
        {
            return url?.StartsWith(Definition.RedirectUrl, StringComparison.OrdinalIgnoreCase) == true;
        }

        internal async Task<OAuthResponse> GetTokenFromCode(string code)
        {
            if (string.IsNullOrEmpty(Definition.TokenUrl))
                return OAuthResponse.WithError("BadImplementation",
                    "Provider returns code in authorize request but there is not access token URL.");

            return await GetToken(Definition.TokenUrl,
                Definition.TokenAuthorizationHeaders,
                BuildTokenRequestFields(code));
        }

        internal async Task<OAuthResponse> RefreshToken(OAuthAccessToken token)
        {
            if (!Definition.RefreshesToken)
                return OAuthResponse.WithError("NotSupported",
                   "Provider does not support token refresh.");
            
            return await GetToken(Definition.TokenUrl,
                Definition.RefreshTokenAuthorizationHeaders,
                BuildRefreshTokenRequestFields(
                    string.IsNullOrEmpty(token.RefreshToken) ?
                    token.Token
                    :
                    token.RefreshToken));
        }

        internal bool RefreshesToken()
        {
           return Definition.RefreshesToken;
        }

        internal async Task<AccountData> GetAccountData(OAuthAccessToken token)
        {
            var graphResponseString = await GetResource<string>(Definition.GraphUrl, token);

            return ReadAccountData(graphResponseString);
        }
        #endregion

        #region Protected Members
        protected OAuthProviderDefinition Definition { get { return _definition; } }

        protected string[] GetScopes()
        {
            return Definition.MandatoryScopes.Union(Definition.Scopes ?? new string[0]).Distinct().ToArray();
        }

        protected string GetScopesString(string[] scopes)
        {
            return string.Join(Definition.ScopeSeparator, scopes);
        }

        protected virtual IEnumerable<KeyValuePair<string, string>> BuildTokenRequestFields(string code)
        {
            return BuildRequestFields(GrantType.AuthorizationCode, false, code, Definition.IncludeRedirectUrlInTokenRequest);
        }

        protected virtual IEnumerable<KeyValuePair<string, string>> BuildRefreshTokenRequestFields(string code)
        {
            return BuildRequestFields(GrantType.RefreshToken, Definition.ExcludeClientIdAndSecretInTokenRefresh, code, Definition.IncludeRedirectUrlInRefreshTokenRequest);
        }

        protected static IDictionary<string, string> ReadResponseParameter(string url)
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

        protected static DateTime? GetExpireDate(string value)
        {
            return string.IsNullOrEmpty(value) ?
                (DateTime?)null
                :
                DateTime.Now + TimeSpan.FromSeconds(double.Parse(value));
        }

        protected string BuildUrl(string url, IEnumerable<KeyValuePair<string, string>> queryParameters)
        {
            return url +
               (queryParameters.Any() ?
               "?" + string.Join("&", queryParameters.Select(pair => $"{pair.Key}={pair.Value}"))
               :
               string.Empty);
        }

        protected static KeyValuePair<string, string> BuildBasicAuthenticationHeader(string clientId, string clientSecret)
        {
            return new KeyValuePair<string, string>("Authorization", $"Basic {BuildAuthenticationData(clientId, clientSecret)}");
        }

        protected static string BuildAuthenticationData(string clientId, string clientSecret)
        {
            var data = $"{clientId}:{clientSecret}";

            var dataBytes = Encoding.UTF8.GetBytes(data);

            return Convert.ToBase64String(dataBytes);
        }
        #endregion

        #region Private Methods
        private async Task<OAuthResponse> GetToken(string url,
            IEnumerable<KeyValuePair<string, string>> headers,
            IEnumerable<KeyValuePair<string, string>> fields)
        {
            using (var tokenClient = new HttpClient())
            {
                foreach (var header in headers)
                    tokenClient.DefaultRequestHeaders.Add(header.Key, header.Value);

                tokenClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var tokenResponse = await tokenClient.PostAsync(url, BuildHttpContent(fields));

                var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();

                return GetOAuthResponseFromJson(tokenResponseString);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> BuildRequestFields(GrantType grantType, bool excludeClientIdAndSecret, string code, bool includeRedirectUrl)
        {
            var fields = new Dictionary<string, string>
            {
                { "grant_type", grantType == GrantType.AuthorizationCode ? "authorization_code" : "refresh_token" },
                { grantType == GrantType.AuthorizationCode ? "code" : "refresh_token", code }
            };

            if (includeRedirectUrl)
                fields.Add("redirect_uri", WebUtility.UrlEncode(Definition.RedirectUrl));

            if (!excludeClientIdAndSecret)
            {
                if (!Definition.ExcludeClientIdInTokenRequest)
                    fields.Add("client_id", Definition.ClientId);

                if (!string.IsNullOrEmpty(Definition.ClientSecret) && !Definition.ExcludeClientSecretInTokenRequest)
                    fields.Add("client_secret", Definition.ClientSecret);
            }

            return fields;
        }

        private static HttpContent BuildHttpContent(IEnumerable<KeyValuePair<string, string>> fields)
        {
            var content = string.Join("&", fields.Select(pair => $"{pair.Key}={pair.Value}"));
            return new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        private OAuthResponse GetOAuthResponseFromJson(string json)
        {
            var jObject = JObject.Parse(json);

            var error = jObject.GetStringValue(_errorParameter);
            if (!string.IsNullOrEmpty(error))
                return OAuthResponse.WithError(error, jObject.GetStringValue(_errorDescriptionParameter));

            return OAuthResponse.WithToken(new OAuthAccessToken(
                jObject.GetStringValue(_accessTokenParatemeter),
                "bearer".Equals(jObject.GetStringValue(_tokenTypeParameter), StringComparison.OrdinalIgnoreCase) ?
                    TokenType.Bearer
                    :
                    TokenType.Unknown,
                jObject.GetStringValue(_refreshTokenParameter),
                GetExpireDate(jObject.GetStringValue(_expiresInParameter))));
        }

        private string BuildResourceTokenUrl(string url, OAuthAccessToken token, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
        {
            var parameters = new List<KeyValuePair<string, string>>(Definition.ResourceQueryParameters);

            if (null != queryParameters)
                parameters.AddRange(queryParameters);

            if (Definition.TokenType == TokenType.Url)
                parameters.Add(new KeyValuePair<string, string>(Definition.TokenRequestUrlParameter, token.Token));

            return BuildUrl(url, parameters);
        }

        private static async Task<T> ReadHttpResponse<T>(HttpResponseMessage response)
            where T : class
        {
            var responseString = await ReadContent(response.Content);

            if (typeof(T) == typeof(string))
                return responseString as T;

            if (typeof(T) == typeof(JArray))
                return Task.FromResult(JArray.Parse(responseString)) as T;

            if (typeof(T) == typeof(JObject))
                return Task.FromResult(JObject.Parse(responseString)) as T;

            return JsonConvert.DeserializeObject<T>(responseString);
        }

        private enum GrantType
        {
            AuthorizationCode,
            RefreshToken
        }
        #endregion
    }
}