namespace Xamarin.Forms.OAuth
{
    public class OAuthAccount
    {
        internal OAuthAccount(string id, string displayName, string provider, string token)
        {
            Id = id;
            DisplayName = displayName;
            Provider = provider;
            AccessToken = token;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string Provider { get; private set; }
        public string AccessToken { get; private set; }

        public override string ToString()
        {
            return $"{Provider}: {DisplayName} ({Id})";
        }
    }
}
