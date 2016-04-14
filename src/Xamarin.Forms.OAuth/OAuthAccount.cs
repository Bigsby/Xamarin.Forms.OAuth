namespace Xamarin.Forms.OAuth
{
    public class OAuthAccount
    {
        internal OAuthAccount(string id, string displayName, OAuthProvider provider, OAuthAccessToken token)
        {
            Id = id;
            DisplayName = displayName;
            ProviderInstance = provider;
            AccessToken = token;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string Provider { get { return ProviderInstance.Name; } }
        public OAuthAccessToken AccessToken { get; private set; }
        internal OAuthProvider ProviderInstance { get; private set; }

        internal void SetToken(OAuthAccessToken token)
        {
            AccessToken = token;
        }

        public override string ToString()
        {
            return $"{Provider}: {DisplayName} ({Id})";
        }


    }
}
