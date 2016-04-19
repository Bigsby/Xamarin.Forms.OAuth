using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xamarin.Forms.OAuth.Providers
{
    public class ReddItOAuthProvider : OAuthProvider
    {
        //TODO: add duration parameter in Authorize to get refresh_token
        internal ReddItOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
            : base(clientId, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "ReddIt";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://www.reddit.com/api/v1/authorize";
            }
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://oauth.reddit.com/api/v1/me";
            }
        }

        protected override string[] MandatoryScopes
        {
            get
            {
                return new[] { "identity" };
            }
        }

        protected override bool RequiresCode
        {
            get
            {
                return true;
            }
        }

        internal override string TokenUrl
        {
            get
            {
                return "https://www.reddit.com/api/v1/access_token";
            }
        }

        protected override bool IncludeStateInAuthorize
        {
            get
            {
                return true;
            }
        }

        protected override bool IncludeRedirectUrlInTokenRequest
        {
            get
            {
                return true;
            }
        }

        protected override bool ExcludeClientIdInTokenRequest
        {
            get
            {
                return true;
            }
        }

        internal override string TokenAuthorizationHeader
        {
            get
            {
                return $"Basic {BuildAuthenticationData()}";
            }
        }

        private string BuildAuthenticationData()
        {
            var data = $"{ClientId}:{ClientSecret}";

            var dataBytes = Encoding.UTF8.GetBytes(data);

            return Convert.ToBase64String(dataBytes);
        }

        protected override TokenType TokenType
        {
            get
            {
                return TokenType.Bearer;
            }
        }

        internal override IEnumerable<KeyValuePair<string, string>> ResourceHeaders(OAuthAccessToken token)
        {
            return base.ResourceHeaders(token).Union(new[] 
            {
                //TODO: build User-Agent according to documentation
                //https://github.com/reddit/reddit/wiki/API
                new KeyValuePair<string, string>("User-Agent", Guid.NewGuid().ToString()),
            });
        }
    }
}
