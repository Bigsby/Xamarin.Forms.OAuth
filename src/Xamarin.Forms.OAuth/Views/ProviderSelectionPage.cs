using System.Windows.Input;
using Xamarin.Forms.OAuth.ViewModels;

namespace Xamarin.Forms.OAuth.Views
{
    internal sealed class ProviderSelectionPage : BaseView, IBackHandlingView
    {
        private readonly ProviderSelectionViewModel _viewModel;

        public ProviderSelectionPage(ProviderSelectionViewModel viewModel)
            : base()
        {
            BindingContext = _viewModel = viewModel;
            var grid = new Grid();

            var label = new Label
            {
                Text = OAuthAuthenticator.ProviderSelectText,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            var fontSize = Device.GetNamedSize(NamedSize.Large, label);

            grid.Children.Add(new Button
            {
                Text = OAuthAuthenticator.BackButtonText,
                Command = viewModel.Cancel
            });

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(fontSize * 2) });
            grid.RowDefinitions.Add(new RowDefinition());

            Grid.SetRow(label, 1);
            label.FontSize = fontSize;

            grid.Children.Add(label);

            var providerStack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = fontSize
            };

            foreach (var provider in viewModel.Providers)
            {
                var button = new ProviderButton
                {
                    Command = BuildSelectionHandler()
                };
                button.SetBinding(ProviderButton.TextProperty, OAuthProvider.NameProperty, BindingMode.OneWay);
                button.SetBinding(ProviderButton.ImageProperty, OAuthProvider.LogoProperty, BindingMode.OneWay);
                providerStack.Children.Add(button);
                button.BindingContext = provider;
            }

            var scrollView = new ScrollView
            {
                Content = providerStack
            };

            Grid.SetRow(scrollView, 2);
            grid.Children.Add(scrollView);

            Padding = fontSize;
            Content = grid;
        }

        public ICommand BuildSelectionHandler()
        {
            return new Command(p => _viewModel.SelectedProvider = (OAuthProvider)p);
        }

        public void HandleBack()
        {
            _viewModel.Cancel.Execute(null);
        }
    }

    internal class ProviderButton : Grid
    {
        private readonly Label _text;
        private readonly Image _image;
        private const double _imageMargin = 2;
        private static Color _background = Color.FromHex("CCCCCC");
        private static Color _textColor = Color.FromHex("000000");
        private static double _fontSize = Device.GetNamedSize(NamedSize.Medium, new Label());

        public ProviderButton()
        {
            BackgroundColor = _background;

            _text = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                TextColor = _textColor,
                InputTransparent = true
            };
            //var fontSize = Device.GetNamedSize(NamedSize.Medium, _text);
            _text.FontSize = _fontSize;
            var imageSize = (_fontSize * 2) - _imageMargin;

            _image = new Image
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                HeightRequest = imageSize,
                WidthRequest = imageSize,
                Aspect = Aspect.AspectFit,
                InputTransparent = true
            };

            var stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = _fontSize,
                Orientation = StackOrientation.Horizontal
            };

            stack.Children.Add(_image);
            stack.Children.Add(_text);

            Children.Add(stack);

            GestureRecognizers.Add(BuildTapRecognizer());
        }

        public static BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(ProviderButton), string.Empty);
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static BindableProperty ImageProperty = BindableProperty.Create("Image", typeof(ImageSource), typeof(ProviderButton), null);
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public ICommand Command { get; set; }

        internal static void SetBackground(Color color)
        {
            _background = color;
        }

        internal static void SetTextColor(Color color)
        {
            _textColor = color;
        }

        internal static void SetFontSize(double size)
        {
            _fontSize = size;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            _text.Text = Text;
            _image.Source = Image;
        }

        private TapGestureRecognizer BuildTapRecognizer()
        {
            var recongnizer = new TapGestureRecognizer();

            recongnizer.Tapped += (s, e) => Command?.Execute(BindingContext);

            return recongnizer;
        }
    }
}
