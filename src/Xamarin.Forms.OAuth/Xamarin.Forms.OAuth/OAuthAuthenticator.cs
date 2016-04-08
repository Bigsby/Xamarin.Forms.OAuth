using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin.Forms.OAuth
{
    public static class OAuthAuthenticator
    {
        private static readonly ICollection<OAuthProvider> _providers = new List<OAuthProvider>();
        private static readonly ManualResetEvent _awaiter = new ManualResetEvent(false);

        public static void AddPRovider(OAuthProvider provider)
        {
            _providers.Add(provider);
        }

        public static async Task<AuthenticatonResult> Authenticate()
        {
            if (!_providers.Any())
                return AuthenticatonResult.Failed("No providers");

            var selectionPage = new ProviderSelectionPage(_providers);


            return null;
        }
    }
}
