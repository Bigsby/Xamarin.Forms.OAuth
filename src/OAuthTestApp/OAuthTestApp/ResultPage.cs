using System;
using Xamarin.Forms;
using Xamarin.Forms.OAuth;
using Xamarin.Forms.OAuth.Views;

namespace OAuthTestApp
{
    public class ResultPage : ContentPage, IBackHandlingView
    {
        private readonly Action _returnCallback;
        private readonly Label _expireLabel = new Label();
        private readonly OAuthAccount _account;

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
                _account = result.Account;

                stack.Children.Add(new Label
                {
                    Text = $"Provider: {_account.ProviderName}"
                });

                stack.Children.Add(new Label
                {
                    Text = $"Id: {_account.Id}"
                });

                stack.Children.Add(new Label
                {
                    Text = $"Name: {_account.DisplayName}"
                });

                var token = _account.AccessToken;

                if (token.Expires.HasValue)
                {
                    SetExpireText();
                    stack.Children.Add(_expireLabel);
                }

                if (result.Account.RefreshesToken)
                    stack.Children.Add(new Button
                    {
                        Text = "Refresh Token",
                        Command = new Command(async () =>
                        {
                            var response = await result.Account.RefreshToken();

                            if (response)
                                SetExpireText();
                        })
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

        private void SetExpireText()
        {
            _expireLabel.Text = $"Token Expires: " 
                + (_account.AccessToken.Expires.HasValue ?
                $"{_account.AccessToken.Expires?.ToString("yyyy-MM-dd HH:mm:ss")}"
                :
                "N/A");
        }
    }
}
