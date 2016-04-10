using System;
using Xamarin.Forms;
using Xamarin.Forms.OAuth;

namespace OAuthTestApp
{
    public class ResultPage : ContentPage
    {
        public ResultPage(OAuthAccount account, Action returnCallback)
        {
            var stack = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            stack.Children.Add(new Label
            {
                Text = $"Provider: {account.Provider}"
            });

            stack.Children.Add(new Label
            {
                Text = $"Id: {account.Id}"
            });

            stack.Children.Add(new Label
            {
                Text = $"Name: {account.DisplayName}"
            });

            stack.Children.Add(new Label
            {
                Text = $"Access Token: {account.AccessToken}"
            });

            stack.Children.Add(new Button
            {
                Text = "Back",
                Command = new Command(returnCallback)
            });
            Content = stack;
        }
    }
}
