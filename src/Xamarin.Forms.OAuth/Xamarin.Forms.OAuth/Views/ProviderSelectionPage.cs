using Xamarin.Forms.OAuth.ViewModels;

namespace Xamarin.Forms.OAuth.Views
{
    internal sealed class ProviderSelectionPage : ContentPage
    {
        public ProviderSelectionPage(ProviderSelectionViewModel viewModel)
        {
            BindingContext = viewModel;
            var stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            var label = new Label
            {
                Text = "Select Provider",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            var fontSize = Device.GetNamedSize(NamedSize.Large, label);
            label.FontSize = fontSize;

            stack.Children.Add(label);

            var listView = new ListView
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            var template = new DataTemplate(typeof(ImageCell));
            template.SetBinding(TextCell.TextProperty, OAuthProvider.NameProperty);
            template.SetBinding(ImageCell.ImageSourceProperty, OAuthProvider.LogoProperty);
            listView.ItemTemplate = template;

            listView.SetBinding(ListView.ItemsSourceProperty, "Providers", BindingMode.OneWay);
            listView.SetBinding(ListView.SelectedItemProperty, "SelectedProvider", BindingMode.OneWayToSource);
            stack.Children.Add(listView);

            Content = stack;
        }
    }
}
