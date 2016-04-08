using System;
using Xamarin.Forms.OAuth.ViewModels;

namespace Xamarin.Forms.OAuth
{
    internal sealed class ProviderSelectionPage : ContentPage
    {
        public ProviderSelectionPage(ProviderSelectionViewModel viewModel)
        {
            BindingContext = viewModel;
            var grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var label = new Label { Text = "Select Provider" };
            grid.Children.Add(label);
            
            var listView = new ListView
            {
                ItemTemplate = BuildDataTemplate(),
            };
            Grid.SetRow(listView, 1);
            listView.SetBinding(ListView.ItemsSourceProperty, "Providers", BindingMode.OneWay);
            listView.SetBinding(ListView.SelectedItemProperty, "SelectedProvider", BindingMode.OneWayToSource);
            
            grid.Children.Add(listView);

            Content = grid;
        }

        private DataTemplate BuildDataTemplate()
        {
            var dataTemplate = new DataTemplate(typeof(TextCell));

            dataTemplate.SetBinding(TextCell.TextProperty, "Name");

            return dataTemplate;
        }
    }


}
