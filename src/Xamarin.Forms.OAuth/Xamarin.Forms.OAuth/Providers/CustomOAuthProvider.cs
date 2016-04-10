using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.OAuth.Providers
{
    internal class CustomOAuthProvider : OAuthProvider
    {
        private readonly string _name;
        private readonly string _authoizationUrl;
        private readonly string _redirectUrl;
        private readonly string _graphUrl;
        private ImageSource _logo;

        public CustomOAuthProvider(
            string name,
            string authorizationUrl,
            string redirectUrl,
            string graphUrl,
            string clientId,
            ImageSource logo = null)
            : base(clientId, redirectUrl)
        {
            _name = name;
            _authoizationUrl = authorizationUrl;
            _redirectUrl = redirectUrl;
            _graphUrl = graphUrl;
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
    }
}
