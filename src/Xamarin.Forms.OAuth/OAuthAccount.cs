using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xamarin.Forms.OAuth
{
    public class OAuthAccount
    {
        internal OAuthAccount(string id, string displayName, OAuthProvider provider, OAuthAccessToken token)
        {
            Id = id;
            DisplayName = displayName;
            Provider = provider;
            AccessToken = token;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string ProviderName { get { return Provider.Name; } }
        public bool RefreshesToken { get { return Provider.RefreshesToken(); } }
        public OAuthAccessToken AccessToken { get; private set; }
        private OAuthProvider Provider { get; set; }

        public async Task<T> GetResource<T>(string resourceUrl, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
            where T : class
        {
            return await Provider.GetResource<T>(resourceUrl, AccessToken, queryParameters);
        }

        public async Task<T> PostResource<T>(string resourceUrl, HttpContent content, IEnumerable<KeyValuePair<string, string>> queryParameters = null)
            where T : class
        {
            return await Provider.PostResource<T>(resourceUrl, content, AccessToken, queryParameters);
        }

        public async Task<OAuthResponse> RefreshToken()
        {
            var result = await Provider.RefreshToken(AccessToken);

            if (result)
                AccessToken = result.Token;

            return result;
        }

        internal void SetToken(OAuthAccessToken token)
        {
            AccessToken = token;
        }

        public override string ToString()
        {
            return $"{ProviderName}: {DisplayName} ({Id})";
        }
    }
}
