namespace Xamarin.Forms.OAuth
{
    public class OAuthResponse
    {
        #region Constructors
        private OAuthResponse(bool success, bool cancelled = false)
        {
            Success = success;
            IsCancelled = cancelled;
        }

        private OAuthResponse(OAuthAccessToken token)
            : this(true)
        {
            Token = token;
        }

        private OAuthResponse(string error, string errorDescription)
            : this(false)
        {
            Error = error;
            ErrorDescription = errorDescription;
        }

        private OAuthResponse(string code)
            : this(true)
        {
            Code = code;
            IsCode = true;
        }
        #endregion

        #region Public Properties
        public bool Success { get; private set; }
        public bool IsCancelled { get; private set; }
        public bool IsCode { get; private set; }
        public string Code { get; private set; }
        public OAuthAccessToken Token { get; private set; }
        public string Error { get; private set; }
        public string ErrorDescription { get; private set; }
        #endregion

        #region Static Initializers
        public static implicit operator bool(OAuthResponse response)
        {
            return response.Success;
        }

        public static OAuthResponse WithToken(OAuthAccessToken token)
        {
            return new OAuthResponse(token);
        }

        public static OAuthResponse WithCode(string code)
        {
            return new OAuthResponse(code);
        }

        public static OAuthResponse WithError(string error, string errorDescription)
        {
            return new OAuthResponse(error, errorDescription);
        }

        public static OAuthResponse Cancel()
        {
            return new OAuthResponse(false, false);
        } 
        #endregion
    }
}
