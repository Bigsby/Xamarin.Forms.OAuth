using Windows.UI.Core;

namespace OAuthTestApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new OAuthTestApp.App());
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => e.Handled = OAuthTestApp.App.HandleBackButton();
        }
    }
}
