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
        public string Name { get { return _definition.Name; } }
        public ImageSource Logo
        {
            get
            {
                return _definition.Logo ?? ImageSource.FromResource($"{GetType().Namespace}.Logos.{Name}.png", GetType().GetTypeInfo().Assembly);
            }
        }

        internal virtual async Task<T> GetResource<T>(string resourceUrl, OAuthAccessToken token, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
            where T : class
        {
            var url = BuildResourceTokenUrl(resourceUrl, token.Token, queryParameters);

            using (var client = new HttpClient())
            {
                var graphHeaders = ResourceHeaders(token);

                foreach (var header in graphHeaders)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);

                var response = await client.GetAsync(url);

                return await ReadHttpResponse<T>(response);
            }
        }

        internal virtual async Task<T> PostResource<T>(string resourceUrl, HttpContent content, OAuthAccessToken token, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
            where T : class
        {
            var url = BuildResourceTokenUrl(resourceUrl, token.Token, queryParameters);

            using (var client = new HttpClient())
            {
                var graphHeaders = ResourceHeaders(token);

                foreach (var header in graphHeaders)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);

                var response = await client.PostAsync(url, content);

                return await ReadHttpResponse<T>(response);
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
        internal virtual OAuthResponse GetOAuthResponseFromUrl(string url)
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

            return OAuthResponse.WithToken(new OAuthAccessToken(
                    parameters[_accessTokenParatemeter],
                    GetExpireDate(parameters.ContainsKey(_expiresInParameter) ?
                        parameters[_expiresInParameter]
                        :
                        string.Empty)
                ));
        }

        private OAuthResponse GetOAuthResponseFromJson(string json)
        {
            var jObject = JObject.Parse(json);

            var error = jObject.GetStringValue(_errorParameter);
            if (!string.IsNullOrEmpty(error))
                return OAuthResponse.WithError(error, jObject.GetStringValue(_errorDescriptionParameter));

            return OAuthResponse.WithToken(new OAuthAccessToken(
                jObject.GetStringValue(_accessTokenParatemeter),
                jObject.GetStringValue(_refreshTokenParameter),
                GetExpireDate(jObject.GetStringValue(_expiresInParameter))));
        }

        internal virtual OAuthResponse GetTokenResponse(string response)
        {
            switch (_definition.TokenResponseSerialization)
            {
                case TokenResponseSerialization.JSON:
                    return GetOAuthResponseFromJson(response);            
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
                jObject.GetStringValue(_definition.GraphIdProperty),
                jObject.GetStringValue(_definition.GraphNameProperty));
        }

        internal virtual string GetAuthorizationUrl()
        {
            var scopesToInject = _definition.MandatoryScopes.Union(_definition.Scopes ?? new string[0]).Distinct().ToArray();
            var scope = scopesToInject.Any() ?
                "&scope=" + string.Join(_definition.ScopeSeparator, scopesToInject)
                :
                string.Empty;

            var state = _definition.IncludeStateInAuthorize ?
                "&state=authentication"
                :
                string.Empty;

            return $"{_definition.AuthorizeUrl}?response_type={_definition.AuthorizeResponseType}&client_id={_definition.ClientId}&redirect_uri={WebUtility.UrlEncode(_definition.RedirectUrl)}{scope}{state}";
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

            if (_definition.IncludeRedirectUrlInTokenRequest)
                fields.Add("redirect_uri", WebUtility.UrlEncode(_definition.RedirectUrl));

            if (!_definition.ExcludeClientIdInTokenRequest)
                fields.Add("client_id", _definition.ClientId);

            if (!string.IsNullOrEmpty(_definition.ClientSecret))
                fields.Add("client_secret", _definition.ClientSecret);

            return fields;
        }

        internal virtual string BuildGraphUrl(string token)
        {
            switch (_definition.TokenType)
            {
                case TokenType.Bearer:
                    return _definition.GraphUrl;
                default:
                    return BuildResourceTokenUrl(_definition.GraphUrl, token);
            }
        }

        internal virtual IEnumerable<KeyValuePair<string, string>> ResourceHeaders(OAuthAccessToken token)
        {
            return _definition.TokenType == TokenType.Url ?
                new KeyValuePair<string, string>[0]
                :
                new[] { new KeyValuePair<string, string>("Authorization", $"Bearer {token.Token}") };
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
            return url?.StartsWith(_definition.RedirectUrl) == true;
        }

        internal async Task<OAuthResponse> GetTokenFromCode(string code)
        {
            if (string.IsNullOrEmpty(_definition.TokenUrl))
                return OAuthResponse.WithError("BadImplementation",
                    "Provider returns code in authorize request but there is not access token URL.");

            using (var tokenClient = new HttpClient())
            {
                if (Definition.TokenAuthorizationHeaders.Any())
                    foreach (var header in Definition.TokenAuthorizationHeaders)
                        tokenClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    //tokenClient.DefaultRequestHeaders.Add("Authorization", Definition.TokenAuthorizationHeader);

                tokenClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var tokenResponse = await tokenClient.PostAsync(_definition.TokenUrl,
                    BuildHttpContent(BuildTokenContent(code)));

                var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();

                return GetOAuthResponseFromJson(tokenResponseString);
            }
        }

        internal async Task<AccountData> GetAccountData(OAuthAccessToken token)
        {
            var graphResponseString = await GetResource<string>(_definition.GraphUrl, token);

            return GetAccountData(graphResponseString);
        }
        #endregion

        #region Protected Members
        protected OAuthProviderDefinition Definition { get { return _definition; } }

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

        protected static DateTime GetExpireDate(string value)
        {
            return string.IsNullOrEmpty(value) ?
                DateTime.MinValue
                :
                DateTime.Now + TimeSpan.FromSeconds(double.Parse(value));
        }
        #endregion

        #region Private Methods
        private static HttpContent BuildHttpContent(string content)
        {
            return new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        private string BuildResourceTokenUrl(string url, string token, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
        {
            var parameters = new List<KeyValuePair<string, string>>(_definition.ResourceQueryParameters);

            if (null != queryParameters)
                parameters.AddRange(queryParameters);

            if (_definition.TokenType == TokenType.Url)
                parameters.Add(new KeyValuePair<string, string>(_definition.TokeUrlParameter, token));

            return url +
                (parameters.Any() ?
                "?" + string.Join("&", parameters.Select(pair => $"{pair.Key}={pair.Value}"))
                :
                string.Empty);
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
        #endregion
    }
}
