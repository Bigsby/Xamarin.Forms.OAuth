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
                Command = new Command(async () => HandleResult(await OAuthAuthenticator.Authenticate()))
            };
        }

        private void HandleResult(AuthenticatonResult result)
        { }
    }
}
