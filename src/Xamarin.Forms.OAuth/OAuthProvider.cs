using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Xamarin.Forms.OAuth
{
    public abstract class OAuthProvider
    {
        private readonly string[] _scopes;

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

        protected string ClientId { get; private set; }
        protected string ClientSecret { get; private set; }
        protected virtual string[] MandatoryScopes { get { return new string[0]; } }
        internal string RedirectUrl { get; private set; }

        private const string _errorParameter = "error";
        private const string _errorDescriptionParameter = "error_description";
        private const string _codeParameter = "code";
        private const string _accessTokenParatemeter = "access_token";
        private const string _refreshTokenParameter = "refresh_token";
        private const string _expiresInParameter = "expires_in";

        internal virtual OAuthResponse GetOAuthResponseFromUrl(string url)
        {
            var parameters = ReadReponseParameter(url);

            if (parameters.ContainsKey(_errorParameter))
                return new OAuthResponse(parameters[_errorParameter],
                    parameters.ContainsKey(_errorDescriptionParameter) ?
                        parameters[_errorDescriptionParameter]
                        :
                        null
                    );

            if (parameters.ContainsKey(_codeParameter))
                return new OAuthResponse(parameters[_codeParameter]);

            return new OAuthResponse(new OAuthAccessToken(
                    parameters[_accessTokenParatemeter],
                    parameters.ContainsKey(_expiresInParameter) ?
                        DateTime.Now + TimeSpan.FromSeconds(double.Parse(parameters[_expiresInParameter]))
                        :
                        DateTime.MinValue
                ));
        }

        internal virtual OAuthResponse GetOAuthResponseFromJson(string json)
        {
            var jObject = JObject.Parse(json);

            var error = jObject.GetStringValue(_errorParameter);
            if (!string.IsNullOrEmpty(error))
                return new OAuthResponse(error, jObject.GetStringValue(_errorDescriptionParameter));

            return new OAuthResponse(new OAuthAccessToken(
                jObject.GetStringValue(_accessTokenParatemeter),
                jObject.GetStringValue(_refreshTokenParameter),
                GetExpireDate(jObject.GetStringValue(_expiresInParameter))));
        }

        internal virtual Tuple<string, string> GetAccountData(string json)
        {
            var jObject = JObject.Parse(json);
            return new Tuple<string, string>(
                jObject.GetStringValue(IdPropertyName),
                jObject.GetStringValue(NamePropertyName));
        }

        internal string GetAuthorizationUrl()
        {
            var scopesToInject = MandatoryScopes.Union(_scopes ?? new string[0]).Distinct().ToArray();
            var scope = scopesToInject.Any() ?
                "&scope=" + string.Join(",", scopesToInject)
                :
                string.Empty;

            var responseType = RequireCode ? "code" : "token";

            return $"{AuthorizeUrl}?response_type={responseType}&client_id={ClientId}&redirect_uri={WebUtility.UrlEncode(RedirectUrl)}{scope}";
        }

        internal string BuildTokenContent(string code)
        {
            return $"grant_type=authorization_code&code={code}&client_id={ClientId}&client_secret={ClientSecret}";
        }

        internal string BuildGraphUrl(string token)
        {
            return $"{GraphUrl}?access_token={token}";
        }

        protected abstract string AuthorizeUrl { get; }
        internal virtual string TokenUrl { get { return string.Empty; } }
        protected abstract string GraphUrl { get; }

        internal virtual string IdPropertyName { get { return "id"; } }
        internal virtual string NamePropertyName { get { return "name"; } }

        internal virtual bool RequireCode { get { return false; } }

        //private static Regex BuildJsonValueRegex(string name)
        //{
        //    return new Regex($"\"{name}\".*?:.*?\"([^\"]*)\"", RegexOptions.IgnoreCase);
        //}

        private static Regex _urlParameterExpression = new Regex("(.*)=(.*)");
        private static IDictionary<string, string> ReadReponseParameter(string url)
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

        //private static string GetJsonValue(string json, string name)
        //{
        //    var match = BuildJsonValueRegex(name).Match(json);

        //    return match.Success ? match.Groups[1].Value : string.Empty;
        //}

        private static DateTime GetExpireDate(string value)
        {
            return string.IsNullOrEmpty(value) ?
                DateTime.MinValue
                :
                DateTime.Now + TimeSpan.FromSeconds(double.Parse(value));
        }
    }

    internal static class JObjectExtension
    {
        public static string GetStringValue(this JObject jObject, string propertyName)
        {
            var token = jObject.GetValue(propertyName);

            return null == token ? string.Empty : token.ToString();
        }
    }
}
