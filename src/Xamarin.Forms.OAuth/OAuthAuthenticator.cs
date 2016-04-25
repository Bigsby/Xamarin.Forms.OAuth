using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private static TypeInfo _providersType;
        #endregion

        #region Public Methods
        public static void SetProviderSelectText(string text)
        {
            ProviderSelectText = text;
        }

        public static void SetBackButtonText(string text)
        {
            BackButtonText = text;
        }

        public static void SetProviderButtonBackground(Color color)
        {
            ProviderButton.SetBackground(color);
        }

        public static void SetProviderButtonTextColor(Color color)
        {
            ProviderButton.SetTextColor(color);
        }

        public static void SetProviderButtonFontSize(double size)
        {
            ProviderButton.SetFontSize(size);
        }

        public static OAuthProvider AddPRovider(OAuthProvider provider)
        {
            _providers.Add(provider);
            return provider;
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
                    Device.BeginInvokeOnMainThread(() => Application.Current.MainPage = new ProgressView());

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

        public static async Task LoadConfiguration(Stream stream)
        {
            _providersType = typeof(OAuthProviders).GetTypeInfo();

            var json = string.Empty;
            JObject providerConfigs = null;

            try
            {
                json = await new StreamReader(stream).ReadToEndAsync();
                providerConfigs = JObject.Parse(json);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            foreach (var jToken in providerConfigs)
                try
                {
                    var config = jToken.Value as JObject;

                    var mis = _providersType.GetDeclaredMethods(jToken.Key.TrimEnd('_'));

                    if (!mis.Any())
                        continue;

                    var mi = PickMethodToCall(mis, config);

                    if (null == mi)
                        continue;

                    var args = BuildParameters(mi, config);
                    var provider = mi?.Invoke(null, args) as OAuthProvider;

                    var nameOverride = config.GetStringValue("name");

                    if (null != provider)
                    {
                        if (!string.IsNullOrEmpty(nameOverride))
                            provider.SetName(nameOverride);

                        AddPRovider(provider);
                    }
                }
                catch
                {
                }
        }

        #endregion

        #region Internal Members
        internal static string ProviderSelectText { get; private set; } = "Select Provider";
        internal static string BackButtonText { get; private set; } = "Back";
        #endregion

        #region Private Methods

        private static object[] BuildParameters(MethodInfo mi, JObject configs)
        {
            var parameters = mi.GetParameters();
            var result = new object[parameters.Length];

            for (int paramterIndex = 0; paramterIndex < parameters.Length; paramterIndex++)
            {
                var parameter = parameters[paramterIndex];

                var jValue = configs[parameter.Name];
                var value = null == jValue ? null : jValue.ToObject(parameter.ParameterType);

                result[paramterIndex] = value;
            }

            return result.ToArray();
        }

        private static MethodInfo PickMethodToCall(IEnumerable<MethodInfo> methods, JObject parameters)
        {
            if (methods.Count() == 1)
                return methods.First();

            var hasName = !string.IsNullOrEmpty(parameters.GetStringValue("name"));
            var hasScopes = !string.IsNullOrEmpty(parameters.GetStringValue("scopes"));

            var expectedParameterCount = parameters.Count
                + (hasName ? -1 : 0)
                + (hasScopes ? 0 : 1);

            var mi = methods.First(m => m.GetParameters().Count() == expectedParameterCount);

            return mi;
        }

        private static MethodInfo GetMethodToRun(IEnumerable<MethodInfo> methods, IDictionary<string, object> parameters)
        {
            if (!methods.Any())
                return null;

            if (methods.Count() == 1)
                return methods.First();

            var parameterCount = parameters.Count();

            foreach (var method in methods)
                if (method.GetParameters().Count() == parameterCount)
                    return method;

            return null;
        }

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
