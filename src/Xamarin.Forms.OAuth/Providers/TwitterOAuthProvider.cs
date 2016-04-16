//using System;
//using System.IO;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;

//namespace Xamarin.Forms.OAuth.Providers
//{
//    public class TwitterOAuthProvider : OAuthProvider
//    {
//        private const string _requestTokenUrl = "http://twitter.com/oauth/request_token?oauth_consumer_key={0}&oauth_version=1.1&oauth_callback=oob";
//        private const string _oauth_tokenParameter = "oauth_token";
//        private const string _oauth_token_secretParameter = "oauth_token_secret";
//        private string _oauth_token;
//        private string _oauth_token_secret;
//        private Random _random = new Random();

//        internal TwitterOAuthProvider(string clientId, string redirectUrl, params string[] scopes)
//            : base(clientId, redirectUrl, scopes)
//        { }

//        public override string Name
//        {
//            get
//            {
//                return "Twitter";
//            }
//        }

//        protected override string AuthorizeUrl
//        {
//            get
//            {
//                return "https://api.twitter.com/oauth/authorize";
//            }
//        }

//        protected override string GraphUrl
//        {
//            get
//            {
//                return "https://api.twitter.com/1.1/account/verify_credentials.json";
//            }
//        }

//        internal override string IdPropertyName
//        {
//            get
//            {
//                return "id_str";
//            }
//        }

//        internal override string GetAuthorizationUrl()
//        {
//            //return string.Format(_requestTokenUrl, ClientId, WebUtility.UrlEncode(RedirectUrl));
//            return base.GetAuthorizationUrl();
//        }

//        internal override void PreAuthenticationProcess()
//        {
//            Task.Run(async () =>
//            {

//                var wv = new WebView();
//                var client = new HttpClient();

//                var response = await client.GetAsync(string.Format(_requestTokenUrl, ClientId));

//                var deflateStream = new Ionic.Zlib.GZipStream(await response.Content.ReadAsStreamAsync(), Ionic.Zlib.CompressionMode.Decompress);

//                var responseString = await new StreamReader(deflateStream).ReadToEndAsync();

//                var parameters = ReadReponseParameter("http://abo.com?" + response);

//                _oauth_token = parameters[_oauth_tokenParameter];
//                _oauth_token_secret = parameters[_oauth_token_secretParameter];

//            }).Wait();
//        }

//        private HttpContent BuildRequestContent()
//        {
//            var sb = new StringBuilder();
//            sb.Append("oauth_consumer_key=" + ClientId);
//            sb.Append(",oauth_signature_method=HMAC-SHA1");
//            sb.Append(",oauth_signature=");
//  //          oauth_consumer_key = 9cS99b2406CUpATOeggeA &
//  //oauth_signature_method = HMAC - SHA1 &
//  //oauth_signature = 3e18bafc4c4fd6b23f988bcd1a8c0ab2d65db784
//  //  oauth_timestamp = 1267523137 &
//  //  oauth_nonce = 56e66e9f8bd28b320f86a16407f9911d &
//  //    oauth_version = 1.0 &
//  //    oauth_callback = http://playground.com

//            var result = new StringContent(sb.ToString());

//            return result;
//        }

//        private string GenerateNonce()
//        {
//            var sb = new StringBuilder();
//            for (int i = 0; i < 8; i++)
//            {
//                int g = _random.Next(3);
//                switch (g)
//                {
//                    case 0:
//                        // lowercase alpha
//                        sb.Append((char)(_random.Next(26) + 97), 1);
//                        break;
//                    default:
//                        // numeric digits
//                        sb.Append((char)(_random.Next(10) + 48), 1);
//                        break;
//                }
//            }
//            return sb.ToString();
//        }
//    }
//}
