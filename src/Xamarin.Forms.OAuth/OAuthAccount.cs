namespace Xamarin.Forms.OAuth
{
    public class OAuthAccount
    {
        internal OAuthAccount(string id, string displayName, OAuthProvider provider, OAuthAccessToken token)
        {
            Id = id;
            DisplayName = displayName;
            Provider = provider;
            AccessToken = token;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string ProviderName { get { return Provider.Name; } }
        public OAuthAccessToken AccessToken { get; private set; }
        public OAuthProvider Provider { get; private set; }

        internal void SetToken(OAuthAccessToken token)
        {
            AccessToken = token;
        }

        public override string ToString()
        {
            return $"{ProviderName}: {DisplayName} ({Id})";
        }


    }
}
