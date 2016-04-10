using System;

namespace Xamarin.Forms.OAuth.Views
{
    internal class WebOAuthPage : ContentPage
    {
        private readonly Func<string, bool> _redirectCheck;
        public WebOAuthPage(string url, Func<string, bool> redirectCheck)
        {
            _redirectCheck = redirectCheck;

            var webView = new WebView { Source = url };

            webView.Navigating += (s, e) => HandleNavigating(e.Url);

            Content = webView;
        }

        private void HandleNavigating(string url)
        {
            if (_redirectCheck(url))
                Done?.Invoke(this, new WebOAuthPageDoneEventArgs(url));
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
