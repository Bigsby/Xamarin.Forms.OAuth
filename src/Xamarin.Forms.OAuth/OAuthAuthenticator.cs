using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        private static string _providerSelectText = "Select Provider";

        public static void AddPRovider(OAuthProvider provider)
        {
            _providers.Add(provider);
        }

        public static async Task<AuthenticatonResult> Authenticate(OAuthProvider provider = null)
        {
            if (!_providers.Any())
                return AuthenticatonResult.Failed("No providers");

            return await Task.Run(async () =>
            {
                if (null == provider)
                {
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
                }

                if (null == provider)
                    return AuthenticatonResult.Failed("No provider selected.");

                OAuthResponse oAuthResponse = null;
                var backPressed = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    var webView = new WebOAuthPage(provider.GetAuthorizationUrl(),
                        () => {
                            backPressed = true;
                            _awaiter.Set();
                        },
                        url => CheckRedirect(url, provider));
                    webView.Done += (s, e) =>
                    {
                        oAuthResponse = provider.GetOAuthResponseFromUrl(e.Url);
                        _awaiter.Set();
                    };
                    Application.Current.MainPage = webView;
                });

                _awaiter.WaitOne();
                _awaiter.Reset();

                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsVisible = false);

                if (backPressed)
                    return AuthenticatonResult.Failed("Cancelled");

                if (!oAuthResponse)
                    return AuthenticatonResult.Failed(oAuthResponse.Error, oAuthResponse.ErrorDescription);

                var client = new HttpClient();
                if (oAuthResponse.IsCode)
                {
                    var tokenResponse = await client.PostAsync(provider.TokenUrl, 
                        BuildHttpContent(provider.BuildTokenContent(oAuthResponse.Code)));

                    var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();

                    oAuthResponse = provider.GetOAuthResponseFromJson(tokenResponseString);

                    if (!oAuthResponse)
                        return AuthenticatonResult.Failed(oAuthResponse.Error, oAuthResponse.ErrorDescription);
                }
                
                var response = await client.GetStringAsync(provider.BuildGraphUrl(oAuthResponse.Token.Token));

                var accountData = provider.GetAccountData(response);
                return AuthenticatonResult.Successful(accountData.Item1, accountData.Item2, provider, oAuthResponse.Token);
            });
        }

        private static HttpContent BuildHttpContent(string content)
        {
            return new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        private static bool CheckRedirect(string url, OAuthProvider provider)
        {
            return url?.StartsWith(provider.RedirectUrl) == true;
        }

        public static void SetProviderSelectText(string text)
        {
            if (!string.IsNullOrEmpty(text))
                _providerSelectText = text;
        }

        internal static string ProviderSelectText { get { return _providerSelectText; } }
    }
}
