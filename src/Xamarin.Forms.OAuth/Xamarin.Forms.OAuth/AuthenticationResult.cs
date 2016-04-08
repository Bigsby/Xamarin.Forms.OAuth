namespace Xamarin.Forms.OAuth
{
    public class AuthenticatonResult
    {
        private AuthenticatonResult(bool success)
        {
            Success = success;
        }

        public OAuthAccount Account { get; private set; }

        public static AuthenticatonResult Successful(string id, string displayName, string provider, string token)
        {
            return new AuthenticatonResult(true)
            {
                Account = new OAuthAccount(id, displayName, provider, token)
            };
        }

        public static AuthenticatonResult Failed(string errorMessage)
        {
            return new AuthenticatonResult(false)
            {
                ErrorMessage = errorMessage
            };
        }

        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
    }
}
