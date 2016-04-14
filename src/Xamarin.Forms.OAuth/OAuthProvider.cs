using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Xamarin.Forms.OAuth.Providers;

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
        protected virtual string[] MandatoryScopes { get { return new string[0]; } }
        internal string RedirectUrl { get; private set; }

        private static Regex _tokenRegex = new Regex("^.*access_token=([^&]+)");
        private static Regex _expiresRegex = new Regex("expires_in=(\\d+)");

        internal virtual OAuthAccessToken RetrieveToken(string url)
        {
            return new OAuthAccessToken(
                    _tokenRegex.Match(url).Groups[1].Value,
                    DateTime.Now + TimeSpan.FromSeconds(double.Parse(_expiresRegex.Match(url).Groups[1].Value))
                );
        }

        internal string GetAuthorizationUrl()
        {
            var scopesToInject = MandatoryScopes.Union(_scopes ?? new string[0]).Distinct().ToArray();
            var scope = scopesToInject.Any() ?
                "&scope=" + string.Join(",", scopesToInject)
                :
                string.Empty;

            return $"{AuthoizationUrl}?response_type=token&client_id={ClientId}&redirect_uri={WebUtility.UrlEncode(RedirectUrl)}{scope}";
        }

        internal string BuildGraphUrl(string token)
        {
            return $"{GraphUrl}?access_token={token}";
        }

        protected abstract string AuthoizationUrl { get; }
        protected abstract string GraphUrl { get; }

        internal virtual string IdPropertyName { get { return "id"; } }
        internal virtual string NamePropertyName { get { return "name"; } }

        private static Regex BuildJsonValueRegex(string name)
        {
            return new Regex($"\"{name}\".*?:.*?\"([^\"]*)\"", RegexOptions.IgnoreCase);
        }

        private static string GetJsonValue(string json, string name)
        {
            var match = BuildJsonValueRegex(name).Match(json);

            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
