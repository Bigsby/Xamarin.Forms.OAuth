using System;
using Xamarin.Forms;
using Xamarin.Forms.OAuth;

namespace OAuthTestApp
{
    public class ResultPage : ContentPage
    {
        public ResultPage(AuthenticatonResult result, Action returnCallback)
        {
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
                    Text = $"Access Token: {result.Account.AccessToken}"
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
                    Text = $"Reason: {result.ErrorMessage}"
                });
            }

            stack.Children.Add(new Button
            {
                Text = "Back",
                Command = new Command(returnCallback)
            });

            Content = stack;
        }
    }
}
