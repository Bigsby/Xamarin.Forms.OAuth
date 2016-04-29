namespace Bigs.OAuth
{
    public class AuthenticatonResult
    {
        private AuthenticatonResult(bool success)
        {
            Success = success;
        }

        public OAuthAccount Account { get; private set; }

        public static AuthenticatonResult Successful(string id, string displayName, OAuthProvider provider, OAuthAccessToken token)
        {
            return new AuthenticatonResult(true)
            {
                Account = new OAuthAccount(id, displayName, provider, token)
            };
        }

        public static AuthenticatonResult Failed(string error, string errorDescription = null)
        {
            return new AuthenticatonResult(false)
            {
                Error = error,
                ErrorDescription = errorDescription
            };
        }

        public bool Success { get; private set; }
        public string Error { get; private set; }
        public string ErrorDescription { get; private set; }

        public static implicit operator bool(AuthenticatonResult result)
        {
            return result.Success;
        }
    }
}
