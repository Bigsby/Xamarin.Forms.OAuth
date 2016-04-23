using Xamarin.Forms;
using Xamarin.Forms.OAuth;

namespace OAuthTestApp
{
    internal sealed class StartPage : ContentPage
    {
        public StartPage()
        {
            Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new Button
                    {
                        Text = "Authenticate",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Command = new Command(async () => HandleResult(await OAuthAuthenticator.Authenticate()))
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
