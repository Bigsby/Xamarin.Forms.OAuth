namespace Xamarin.Forms.OAuth.Views
{
    internal abstract class BaseView : ContentPage
    {
        public BaseView()
        {
            Padding = Device.OnPlatform(
                new Thickness(0, 20, 0, 0), 
                new Thickness(0, 5, 0, 0), 
                new Thickness(0, 10, 0, 0));
        }
    }

    public interface IBackHandlingView
    {
        void HandleBack();
    }
}
