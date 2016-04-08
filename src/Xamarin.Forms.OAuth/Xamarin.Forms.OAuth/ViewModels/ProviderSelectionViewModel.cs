using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Xamarin.Forms.OAuth.ViewModels
{
    internal sealed class ProviderSelectionViewModel : BaseViewModel
    {
        private readonly IEnumerable<OAuthProvider> _providers;

        public ProviderSelectionViewModel(IEnumerable<OAuthProvider> providers)
        {
            _providers = providers;
        }

        public IEnumerable<OAuthProvider> Providers { get { return _providers; } }

        public ICommand Cancel
        {
            get { return new Command(() => RaiseProviderSelected(null)); }
        }

        public OAuthProvider SelectedProvider
        {
            //get { return null; }
            set
            {
                if (null == value)
                    return;

                RaiseProviderSelected(value);
            }
        }

        public event EventHandler<ProviderSelectedEventArgs> ProviderSelected;

        private void RaiseProviderSelected(OAuthProvider provider)
        {
            RaiseEvent(ProviderSelected, () => new ProviderSelectedEventArgs(provider));
        }
    }

    internal sealed class ProviderSelectedEventArgs : EventArgs
    {
        public ProviderSelectedEventArgs(OAuthProvider provider)
        {
            Provider = provider;
        }

        public OAuthProvider Provider { get; private set; }
    }
}