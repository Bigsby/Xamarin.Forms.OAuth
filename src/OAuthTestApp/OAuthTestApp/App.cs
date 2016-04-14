using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.OAuth;
using Xamarin.Forms.OAuth.Views;

namespace OAuthTestApp
{
    public class App : Application
    {
        private const string _facebook = "Facebook";
        private const string _google = "Google";
        private const string _microsoft = "Microsoft";

        private static Dictionary<string, AppConfig> _providerConfigs;

        public App()
        {
            LoadProviders();

            // The root page of your application
            MainPage = new StartPage();
        }

        private static void LoadProviders()
        {
            var names = typeof(App).GetTypeInfo().Assembly.GetManifestResourceNames();
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var json = new StreamReader(assembly.GetManifestResourceStream(assembly.GetName().Name + ".KeysLocal.json")).ReadToEnd();
            _providerConfigs = JsonConvert.DeserializeObject<Dictionary<string, AppConfig>>(json);

            if (_providerConfigs.ContainsKey(_facebook))
                OAuthAuthenticator.AddPRovider(OAuthProvider.Facebook(_providerConfigs[_facebook].ClientId));

            if (_providerConfigs.ContainsKey(_google))
                OAuthAuthenticator.AddPRovider(OAuthProvider.Google(_providerConfigs[_google].ClientId,
                    _providerConfigs[_google].RedirectUrl));

            if (_providerConfigs.ContainsKey(_microsoft))
                OAuthAuthenticator.AddPRovider(OAuthProvider.Microsoft(_providerConfigs[_microsoft].ClientId,
                    _providerConfigs[_microsoft].RedirectUrl));
        }

        public static bool HandleBackButton()
        {
            if (!(Current.MainPage is IBackHandlingView))
                return false;

            (Current.MainPage as IBackHandlingView).HandleBack();
            return true;

        }

        internal static AppConfig GetProviderConfig(string providerName)
        {
            return _providerConfigs.ContainsKey(providerName) ?
                _providerConfigs[providerName]
                :
                null;
        }
    }

    internal class AppConfig
    {
        public string ClientId { get; set; }
        public string RedirectUrl { get; set; }
    }
}
