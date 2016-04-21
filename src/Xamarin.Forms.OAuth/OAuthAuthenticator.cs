using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void SetProviderButtonBackground(Color color)
        {
            ProviderButton.SetBackgroundButton(color);
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


                try
                {
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
                        oAuthResponse = await provider.GetTokenFromCode(oAuthResponse.Code);

                        if (!oAuthResponse)
                            return AuthenticatonResult.Failed(oAuthResponse.Error, oAuthResponse.ErrorDescription);
                    }

                    var accountData = await provider.GetAccountData(oAuthResponse.Token);
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
                    url => provider.CheckRedirect(url));
                webView.Done += (s, e) =>
                {
                    oAuthResponse = provider.ReadOAuthResponseFromUrl(e.Url);
                    _awaiter.Set();
                };
                Application.Current.MainPage = webView;
            });

            _awaiter.WaitOne();
            _awaiter.Reset();

            return Task.FromResult(oAuthResponse);
        }
        #endregion
    }
}
