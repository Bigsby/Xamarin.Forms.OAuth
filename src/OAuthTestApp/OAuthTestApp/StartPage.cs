using Xamarin.Forms;
using Xamarin.Forms.OAuth;

namespace OAuthTestApp
{
    internal sealed class StartPage : ContentPage
    {
        public StartPage()
        {
            Content = new Button {
                Text = "Authenticate",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Command = new Command(async () => HandleResult(await OAuthAuthenticator.Authenticate()))
            };
        }

        private void HandleResult(AuthenticatonResult result)
        {
            Device.BeginInvokeOnMainThread(() => Application.Current.MainPage = new ResultPage(
                result,
                async () => HandleResult(await OAuthAuthenticator.Authenticate())));
        }
    }
}
