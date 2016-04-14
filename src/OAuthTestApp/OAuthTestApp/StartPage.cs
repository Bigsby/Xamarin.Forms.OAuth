using Xamarin.Forms;
using Xamarin.Forms.OAuth;

namespace OAuthTestApp
{
    internal sealed class StartPage : ContentPage
    {
        public StartPage()
        {
            var facebookConfig = App.GetProviderConfig("Facebook");
            var googleConfig = App.GetProviderConfig("Google");
            var microsoftConfig = App.GetProviderConfig("Microsoft");

            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new Button
                    {
                        Text = "Built-in Provider List",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Command = new Command(async () => HandleResult(await OAuthAuthenticator.Authenticate()))
                    },
                    new Button
                    {
                        Text = "Facebook",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Command = new Command(async () => HandleResult(await OAuthAuthenticator.Authenticate(OAuthProviders.Facebook(facebookConfig.ClientId))))
                    },
                    new Button
                    {
                        Text = "Google",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Command = new Command(async () => HandleResult(await OAuthAuthenticator.Authenticate(OAuthProviders.Google(googleConfig.ClientId, googleConfig.RedirectUrl))))
                    },
                    new Button
                    {
                        Text = "Microsoft",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Command = new Command(async () => HandleResult(await OAuthAuthenticator.Authenticate(OAuthProviders.Microsoft(microsoftConfig.ClientId, microsoftConfig.RedirectUrl))))
                    }
                }
            };
        }

        private void HandleResult(AuthenticatonResult result)
        {
            Device.BeginInvokeOnMainThread(() => Application.Current.MainPage = new ResultPage(
                result,
                () => Application.Current.MainPage = new StartPage()));
        }
    }
}
