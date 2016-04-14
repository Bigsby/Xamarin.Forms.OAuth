namespace Xamarin.Forms.OAuth.Providers
{
    internal class CustomOAuthProvider : OAuthProvider
    {
        private readonly string _name;
        private readonly string _authoizationUrl;
        private readonly string _redirectUrl;
        private readonly string _graphUrl;
        private readonly string _idProperty;
        private readonly string _nameProperty;

        private ImageSource _logo;

        internal CustomOAuthProvider(
            string name,
            string authorizationUrl,
            string redirectUrl,
            string graphUrl,
            string clientId,
            string idProperty = null,
            string nameProperty = null,
            ImageSource logo = null,
            params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        {
            _name = name;
            _authoizationUrl = authorizationUrl;
            _redirectUrl = redirectUrl;
            _graphUrl = graphUrl;
            _idProperty = idProperty;
            _nameProperty = nameProperty;
            _logo = logo;
        }

        public override string Name
        {
            get
            {
                return _name;
            }
        }

        protected override string AuthoizationUrl
        {
            get
            {
                return _authoizationUrl;
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return _graphUrl;
            }
        }

        public override ImageSource Logo
        {
            get
            {
                return _logo;
            }
        }

        internal override string IdPropertyName
        {
            get
            {
                return string.IsNullOrEmpty(_idProperty) ? base.IdPropertyName : _idProperty;
            }
        }

        internal override string NamePropertyName
        {
            get
            {
                return string.IsNullOrEmpty(_nameProperty) ? base.NamePropertyName : _nameProperty;
            }
        }
    }
}
