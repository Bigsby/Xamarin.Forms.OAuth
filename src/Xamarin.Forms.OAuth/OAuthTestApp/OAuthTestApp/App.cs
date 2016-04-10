using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.OAuth;

namespace OAuthTestApp
{
    public class App : Application
    {
        private const string _facebook = "Facebook";
        private const string _google = "Google";
        private const string _microsoft = "Microsoft";

        public App()
        {
            LoadProviders();

            // The root page of your application
            MainPage = new StartPage();
        }

        private void LoadProviders()
        {
            var names = typeof(App).GetTypeInfo().Assembly.GetManifestResourceNames();
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var json = new StreamReader(assembly.GetManifestResourceStream(assembly.GetName().Name + ".KeysLocal.json")).ReadToEnd();
            var providersConfig = JsonConvert.DeserializeObject<Dictionary<string, AppConfig>>(json);

            if (providersConfig.ContainsKey(_facebook))
                OAuthAuthenticator.AddPRovider(OAuthProvider.Facebook(providersConfig[_facebook].ClientId));

            if (providersConfig.ContainsKey(_google))
                OAuthAuthenticator.AddPRovider(OAuthProvider.Google(providersConfig[_google].ClientId,
                    providersConfig[_google].RedirectUrl));

            if (providersConfig.ContainsKey(_microsoft))
                OAuthAuthenticator.AddPRovider(OAuthProvider.Microsoft(providersConfig[_microsoft].ClientId,
                    providersConfig[_microsoft].RedirectUrl));
        }
    }

    internal class AppConfig
    {
        public string ClientId { get; set; }
        public string RedirectUrl { get; set; }
    }
}
