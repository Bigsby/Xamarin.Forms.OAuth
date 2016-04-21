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
                    parameters[Definition.TokeUrlParameter],
                    GetExpireDate(parameters.ContainsKey(_expiresInParameter) ?
                        parameters[_expiresInParameter]
                        :
                        string.Empty)
                ));
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

        internal virtual OAuthResponse GetTokenResponse(string response)
        {
            switch (Definition.TokenResponseSerialization)
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
            //var scope = scopesToInject.Any() ?
            //    "&scope=" + string.Join(Definition.ScopeSeparator, scopesToInject)
            //    :
            //    string.Empty;

            if (Definition.IncludeStateInAuthorize)
                queryParameters.Add(new KeyValuePair<string, string>("state", "authorization"));

            //var state = Definition.IncludeStateInAuthorize ?
            //    "&state=authentication"
            //    :
            //    string.Empty;

            return BuildUrl(Definition.AuthorizeUrl, queryParameters);
            //return $"{Definition.AuthorizeUrl}?response_type={Definition.AuthorizeResponseType}&client_id={Definition.ClientId}&redirect_uri={WebUtility.UrlEncode(Definition.RedirectUrl)}{scope}{state}";
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

        internal virtual string BuildGraphUrl(string token)
        {
            switch (Definition.TokenType)
            {
                case TokenType.Bearer:
                    return Definition.GraphUrl;
                default:
                    return BuildResourceTokenUrl(Definition.GraphUrl, token);
            }
        }

        internal virtual IEnumerable<KeyValuePair<string, string>> ResourceHeaders(OAuthAccessToken token)
        {
            return Definition.TokenType == TokenType.Url ?
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
            return url?.StartsWith(Definition.RedirectUrl, StringComparison.OrdinalIgnoreCase) == true;
        }

        internal async Task<OAuthResponse> GetTokenFromCode(string code)
        {
            if (string.IsNullOrEmpty(Definition.TokenUrl))
                return OAuthResponse.WithError("BadImplementation",
                    "Provider returns code in authorize request but there is not access token URL.");

            using (var tokenClient = new HttpClient())
            {
                if (Definition.TokenAuthorizationHeaders.Any())
                    foreach (var header in Definition.TokenAuthorizationHeaders)
                        tokenClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                //tokenClient.DefaultRequestHeaders.Add("Authorization", Definition.TokenAuthorizationHeader);

                tokenClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var tokenResponse = await tokenClient.PostAsync(Definition.TokenUrl,
                    BuildHttpContent(BuildTokenContent(code)));

                var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();

                return GetOAuthResponseFromJson(tokenResponseString);
            }
        }

        internal async Task<AccountData> GetAccountData(OAuthAccessToken token)
        {
            var graphResponseString = await GetResource<string>(Definition.GraphUrl, token);

            return GetAccountData(graphResponseString);
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
            var fields = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code }
            };

            if (Definition.IncludeRedirectUrlInTokenRequest)
                fields.Add("redirect_uri", WebUtility.UrlEncode(Definition.RedirectUrl));

            if (!Definition.ExcludeClientIdInTokenRequest)
                fields.Add("client_id", Definition.ClientId);

            if (!string.IsNullOrEmpty(Definition.ClientSecret))
                fields.Add("client_secret", Definition.ClientSecret);

            return fields;
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

        protected static DateTime GetExpireDate(string value)
        {
            return string.IsNullOrEmpty(value) ?
                DateTime.MinValue
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

        private static HttpContent BuildHttpContent(string content)
        {
            return new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        private string BuildResourceTokenUrl(string url, string token, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
        {
            var parameters = new List<KeyValuePair<string, string>>(Definition.ResourceQueryParameters);

            if (null != queryParameters)
                parameters.AddRange(queryParameters);

            if (Definition.TokenType == TokenType.Url)
                parameters.Add(new KeyValuePair<string, string>(Definition.TokeUrlParameter, token));

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
        #endregion
    }
}
