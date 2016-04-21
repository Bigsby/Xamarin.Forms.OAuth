namespace Xamarin.Forms.OAuth.Views
{
    internal sealed class ProgressView : ContentPage
    {
        public ProgressView()
        {
            Content = new ActivityIndicator { IsRunning = true };
        }
    }
}
