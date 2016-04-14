using System;
using Xamarin.Forms;
using Xamarin.Forms.OAuth;
using Xamarin.Forms.OAuth.Views;

namespace OAuthTestApp
{
    public class ResultPage : ContentPage, IBackHandlingView
    {
        private readonly Action _returnCallback;
        public ResultPage(AuthenticatonResult result, Action returnCallback)
        {
            _returnCallback = returnCallback;

            var stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            if (result)
            {
                stack.Children.Add(new Label
                {
                    Text = $"Provider: {result.Account.Provider}"
                });

                stack.Children.Add(new Label
                {
                    Text = $"Id: {result.Account.Id}"
                });

                stack.Children.Add(new Label
                {
                    Text = $"Name: {result.Account.DisplayName}"
                });

                stack.Children.Add(new Label
                {
                    Text = $"Token Expires: {result.Account.AccessToken.Expires.ToString("yyyy-MM-dd HH:mm:ss")}"
                });
            }
            else
            {
                stack.Children.Add(new Label
                {
                    Text = "Authentication failed!"
                });

                stack.Children.Add(new Label
                {
                    Text = $"Reason: {result.Error}"
                });

                if (!string.IsNullOrEmpty(result.ErrorDescription))
                    stack.Children.Add(new Label
                    {
                        Text = $"Description: {result.ErrorDescription}"
                    });
            }

            stack.Children.Add(new Button
            {
                Text = "Back",
                Command = new Command(returnCallback)
            });

            Content = stack;
        }

        public void HandleBack()
        {
            _returnCallback?.Invoke();
        }
    }
}
