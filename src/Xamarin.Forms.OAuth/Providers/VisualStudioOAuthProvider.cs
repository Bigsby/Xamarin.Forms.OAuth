using System.Collections.Generic;
using System.Net;

namespace Xamarin.Forms.OAuth.Providers
{
    public class VisualStudioOAuthProvider : OAuthProvider
    {
        public VisualStudioOAuthProvider(string clientId, string clientSecret, string redirectUrl, params string[] scopes)
            : base(clientId, clientSecret, redirectUrl, scopes)
        { }

        public override string Name
        {
            get
            {
                return "VisualStudio";
            }
        }

        protected override string AuthorizeUrl
        {
            get
            {
                return "https://app.vssps.visualstudio.com/oauth2/authorize";
            }
        }

        protected override string ScopeSeparator
        {
            get
            {
                return " ";
            }
        }

        protected override string AuthorizeResponseType
        {
            get
            {
                return "Assertion";
            }
        }

        protected override bool IncludeStateInAuthorize
        {
            get
            {
                return true;
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
                return "https://app.vssps.visualstudio.com/oauth2/token";
            }
        }

        protected override IEnumerable<KeyValuePair<string, string>> BuildTokenRequestFields(string code)
        {
            return new Dictionary<string, string>
            {
                { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                { "assertion", code },
                { "client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer" },
                { "redirect_uri", WebUtility.UrlEncode(RedirectUrl) },
                { "client_assertion", ClientSecret }
            };
        }

        protected override string GraphUrl
        {
            get
            {
                return "https://app.vssps.visualstudio.com/_apis/profile/profiles/me";
            }
        }

        protected override IEnumerable<KeyValuePair<string, string>> ResourceQueryParameters
        {
            get
            {
                return new[] { new KeyValuePair<string, string>("api-version", "1.0") };
            }
        }

        internal override string GraphNameProperty
        {
            get
            {
                return "displayName";
            }
        }

        protected override TokenType TokenType
        {
            get
            {
                return TokenType.Bearer;
            }
        }
    }
}
