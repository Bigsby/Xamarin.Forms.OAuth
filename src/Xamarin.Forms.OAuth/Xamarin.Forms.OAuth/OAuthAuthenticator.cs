using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.OAuth.ViewModels;
using Xamarin.Forms.OAuth.Views;

namespace Xamarin.Forms.OAuth
{
    public static class OAuthAuthenticator
    {
        private static readonly ICollection<OAuthProvider> _providers = new List<OAuthProvider>();
        private static readonly ManualResetEvent _awaiter = new ManualResetEvent(false);

        public static void AddPRovider(OAuthProvider provider)
        {
            _providers.Add(provider);
        }

        public static async Task<AuthenticatonResult> Authenticate()
        {
            if (!_providers.Any())
                return AuthenticatonResult.Failed("No providers");

            return await Task.Run(async () =>
            {
                OAuthProvider provider = null;

                var selectionViewModel = new ProviderSelectionViewModel(_providers);
                selectionViewModel.ProviderSelected += (s, e) =>
                {
                    provider = e.Provider;
                    _awaiter.Set();
                };

                Device.BeginInvokeOnMainThread(() =>
                    Application.Current.MainPage = new ProviderSelectionPage(selectionViewModel));
                _awaiter.WaitOne();
                _awaiter.Reset();

                if (null == provider)
                    return AuthenticatonResult.Failed("No provider selected.");

                var token = string.Empty;
                
                Device.BeginInvokeOnMainThread(() =>
                {
                    var webView = new WebOAuthPage(provider.GetAuthorizationUrl(), url => CheckRedirect(url, provider));
                    webView.Done += (s, e) =>
                    {
                        token = provider.RetrieveToken(e.Url);
                        _awaiter.Set();
                    };
                    Application.Current.MainPage = webView;
                });

                _awaiter.WaitOne();
                _awaiter.Reset();

                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsVisible = false);

                var client = new HttpClient();
                var response = await client.GetStringAsync(provider.BuildGraphUrl(token));

                var id = GetJsonValue(response, provider.IdPropertyName);
                var name = GetJsonValue(response, provider.NamePropertyName);
                return AuthenticatonResult.Successful(id, name, provider.Name, token);
            });
        }

        private static Regex TokenExpression = new Regex($"^.*access_token=([^&]*)", RegexOptions.IgnoreCase);

        private static string GetJsonValue(string json, string name)
        {
            var match = BuildJsonValueRegex(name).Match(json);

            return match.Success ? match.Groups[1].Value : string.Empty;
        }

        private static Regex BuildJsonValueRegex(string name)
        {
            return new Regex($"\"{name}\".*?:.*?\"([^\"]*)\"", RegexOptions.IgnoreCase);
        }

        private static bool CheckRedirect(string url, OAuthProvider provider)
        {
            return url?.StartsWith(provider.RedirectUrl) == true;
        }
    }
}
