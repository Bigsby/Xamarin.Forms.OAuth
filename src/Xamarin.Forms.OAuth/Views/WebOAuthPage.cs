using System;

namespace Xamarin.Forms.OAuth.Views
{
    internal sealed  class WebOAuthPage : BaseView, IBackHandlingView
    {
        private readonly Func<string, bool> _redirectCheck;
        private readonly Button _backButton;
        private readonly WebView _webView;
        private readonly ActivityIndicator _activityIndicator;
        private readonly Action _back;
        private bool _done = false;

        public WebOAuthPage(string url, Action back, Func<string, bool> redirectCheck)
            : base()
        {
            _redirectCheck = redirectCheck;
            _back = back;

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());

            grid.Children.Add(_backButton = new Button
            {
                Text = "Back",
                Command = new Command(_back),
                IsVisible = false
            });

            _webView = new WebView
            {
                Source = url,
                IsVisible = false
            };

            Grid.SetRow(_webView, 1);
            grid.Children.Add(_webView);
            _webView.Navigating += (s, e) => HandleNavigating(e.Url);
            _webView.Navigated += (s, e) => {
                if (_done) return;
                _webView.IsVisible = true;
                _backButton.IsVisible = true;
                _activityIndicator.IsVisible = false;
            };

            _activityIndicator = new ActivityIndicator { IsRunning = true };
            Grid.SetRowSpan(_activityIndicator, 2);
            grid.Children.Add(_activityIndicator);

            Content = grid;
        }

        private void HandleNavigating(string url)
        {
            if (_redirectCheck(url))
            {
                _webView.IsVisible = false;
                _backButton.IsVisible = false;
                _activityIndicator.IsVisible = true;
                _done = true;
                Done?.Invoke(this, new WebOAuthPageDoneEventArgs(url));
            }
        }

        public void HandleBack()
        {
            _back?.Invoke();
        }

        internal event EventHandler<WebOAuthPageDoneEventArgs> Done;
    }

    internal sealed class WebOAuthPageDoneEventArgs : EventArgs
    {
        internal WebOAuthPageDoneEventArgs(string url)
        {
            Url = url;
        }

        public string Url { get; private set; }
    }
}
