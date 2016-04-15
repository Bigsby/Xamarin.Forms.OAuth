using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var json = new StreamReader(assembly.GetManifestResourceStream(assembly.GetName().Name + ".KeysLocal.json")).ReadToEnd();
            _providerConfigs = JsonConvert.DeserializeObject<Dictionary<string, AppConfig>>(json);

            if (null == _providerConfigs || !_providerConfigs.Any())
                return;

            AddProvidersDynamically();
        }

        private static TypeInfo _providersType;

        private static void AddProvidersDynamically()
        {
            _providersType = typeof(OAuthProviders).GetTypeInfo();

            foreach (var config in _providerConfigs)
            {
                var mi = _providersType.GetDeclaredMethod(config.Key);

                var args = new List<object> { config.Value.ClientId };

                if (!string.IsNullOrEmpty(config.Value.RedirectUrl))
                    args.Add(config.Value.RedirectUrl);

                args.Add(null); // necessary because of parameter > params string[] scopes

                var provider = mi?.Invoke(null, args.ToArray()) as OAuthProvider;

                if (null != provider)
                    OAuthAuthenticator.AddPRovider(provider);
            }
        }

        
        private static MethodInfo GetProviderMethod(string name)
        {
            return _providersType.GetDeclaredMethod(name);
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
