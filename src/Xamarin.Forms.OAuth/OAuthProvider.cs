using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Xamarin.Forms.OAuth.Providers;

namespace Xamarin.Forms.OAuth
{
    public abstract class OAuthProvider
    {
        protected OAuthProvider(string clientId,
            string redirectUrl)
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
                var size = Device.GetNamedSize(NamedSize.Large, new Label());
                return ImageSource.FromResource(
                    typeof(OAuthProvider).GetTypeInfo().Assembly.GetName().Name 
                    + 
                    $".Providers.Logos.{Name}{size}x{size}.png");
            }
        }

        protected string ClientId { get; private set; }
        internal string RedirectUrl { get; private set; }

        internal virtual string RetrieveToken(string url)
        {
            var match = TokenExpression.Match(url);

            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        internal string GetAuthorizationUrl()
        {
            return $"{AuthoizationUrl}&client_id={ClientId}&redirect_uri={WebUtility.UrlEncode(RedirectUrl)}";
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

        public static OAuthProvider Facebook(string appId)
        {
            return new FacebookOAuthProvider(appId);
        }

        public static OAuthProvider Google(string clientId, string redirectUrl)
        {
            return new GoogleOAuthProvider(clientId, redirectUrl);
        }

        public static OAuthProvider Microsoft(string clientId, string redirectUrl)
        {
            return new MicrosoftOAuthProvider(clientId, redirectUrl);
        }

        private static Regex TokenExpression = new Regex($"^.*access_token=([^&]*)", RegexOptions.IgnoreCase);

        private static string GetJsonValue(string json, string name)
        {
            var match = BuildJsonValueRegex(name).Match(json);

            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}
