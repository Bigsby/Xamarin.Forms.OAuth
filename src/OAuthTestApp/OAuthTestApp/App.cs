using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.OAuth;
using Xamarin.Forms.OAuth.Views;

namespace OAuthTestApp
{
    public class App : Application
    {
        public App()
        {
            LoadProviders();

            // The root page of your application
            MainPage = new StartPage();
        }

        private async static void LoadProviders()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".KeysLocal.json");
            await OAuthAuthenticator.LoadConfiguration(stream);
        }

        public static bool HandleBackButton()
        {
            if (!(Current.MainPage is IBackHandlingView))
                return false;

            (Current.MainPage as IBackHandlingView).HandleBack();
            return true;
        }
    }
}
