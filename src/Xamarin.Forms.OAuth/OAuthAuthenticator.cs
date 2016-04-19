using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.OAuth.ViewModels;
using Xamarin.Forms.OAuth.Views;

namespace Xamarin.Forms.OAuth
{
    public static class OAuthAuthenticator
    {
        #region Private Fields
        private static readonly ICollection<OAuthProvider> _providers = new List<OAuthProvider>();
        private static readonly ManualResetEvent _awaiter = new ManualResetEvent(false);
        private static string _providerSelectText = "Select Provider";
        #endregion

        #region Public Methods
        public static void SetProviderSelectText(string text)
        {
            if (!string.IsNullOrEmpty(text))
                _providerSelectText = text;
        }

        public static void AddPRovider(OAuthProvider provider)
        {
            _providers.Add(provider);
        }

        public static async Task<AuthenticatonResult> Authenticate(OAuthProvider provider = null)
        {
            if (null == provider && !_providers.Any())
                return AuthenticatonResult.Failed("No providers");

            return await Task.Run(async () =>
            {
                if (null == provider)
                    provider = await SelectProviderFromList();

                if (null == provider)
                    return AuthenticatonResult.Failed("No provider selected.");

                await provider.PreAuthenticationProcess();

                var oAuthResponse = await AuthenticateUser(provider);

                //TODO show activity indicator at this point
                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsVisible = false);

                if (oAuthResponse.IsCancelled)
                    return AuthenticatonResult.Failed("Cancelled");

                if (!oAuthResponse)
                    return AuthenticatonResult.Failed(oAuthResponse.Error, oAuthResponse.ErrorDescription);


                if (oAuthResponse.IsCode)
                {
                    oAuthResponse = await GetTokenFromCode(provider, oAuthResponse.Code);

                    if (!oAuthResponse)
                        return AuthenticatonResult.Failed(oAuthResponse.Error, oAuthResponse.ErrorDescription);
                }

                try
                {
                    var accountData = await GetAccountData(provider, oAuthResponse.Token);
                    return AuthenticatonResult.Successful(accountData.Id, accountData.Name, provider, oAuthResponse.Token);
                }
                catch (Exception ex)
                {
                    return AuthenticatonResult.Failed(ex.Message, ex.StackTrace);
                }
            });
        }
        #endregion

        #region Internal Members
        internal static string ProviderSelectText { get { return _providerSelectText; } }
        #endregion

        #region Private Methods
        private static Task<OAuthProvider> SelectProviderFromList()
        {
            _awaiter.Reset();
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

            return Task.FromResult(provider);
        }

        private static Task<OAuthResponse> AuthenticateUser(OAuthProvider provider)
        {
            _awaiter.Reset();
            OAuthResponse oAuthResponse = null;

            Device.BeginInvokeOnMainThread(() =>
            {
                var webView = new WebOAuthPage(provider.GetAuthorizationUrl(),
                    () =>
                    {
                        oAuthResponse = OAuthResponse.Cancel();
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

            return Task.FromResult(oAuthResponse);
        }

        private static async Task<OAuthResponse> GetTokenFromCode(OAuthProvider provider, string code)
        {
            if (string.IsNullOrEmpty(provider.TokenUrl))
                return OAuthResponse.WithError("BadImplementation",
                    "Provider returns code in authorize request but there is not access token URL.");

            var tokenClient = new HttpClient();

            if (!string.IsNullOrEmpty(provider.TokenAuthorizationHeader))
                tokenClient.DefaultRequestHeaders.Add("Authorization", provider.TokenAuthorizationHeader);

            tokenClient.DefaultRequestHeaders.Add("Accept", "application/json");

            var tokenResponse = await tokenClient.PostAsync(provider.TokenUrl,
                BuildHttpContent(provider.BuildTokenContent(code)));

            var tokenResponseString = await tokenResponse.Content.ReadAsStringAsync();

            return provider.GetTokenResponse(tokenResponseString);
        }

        private static async Task<OAuthProvider.AccountData> GetAccountData(OAuthProvider provider, OAuthAccessToken token)
        {
            var graphClient = new HttpClient();
            var graphHeaders = provider.GraphHeaders(token);

            foreach (var header in graphHeaders)
                graphClient.DefaultRequestHeaders.Add(header.Key, header.Value);

            var graphResponse = await graphClient.GetAsync(provider.BuildGraphUrl(token.Token));

            var graphResponseString = await ReadContent(graphResponse.Content);

            return provider.GetAccountData(graphResponseString);
        }

        private static async Task<string> ReadContent(HttpContent content)
        {
            if (content.Headers.ContentEncoding.Contains("gzip"))
                return await new StreamReader(new Ionic.Zlib.GZipStream(await content.ReadAsStreamAsync(),
                    Ionic.Zlib.CompressionMode.Decompress
                    )).ReadToEndAsync();

            return await content.ReadAsStringAsync();
        }

        private static HttpContent BuildHttpContent(string content)
        {
            return new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded");
        }

        private static bool CheckRedirect(string url, OAuthProvider provider)
        {
            return url?.StartsWith(provider.RedirectUrl) == true;
        }
        #endregion
    }
}
