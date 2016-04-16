using Newtonsoft.Json.Linq;

namespace Xamarin.Forms.OAuth
{
    internal static class Extensions
    {
        public static string GetStringValue(this JObject jObject, string propertyName)
        {
            var token = jObject.GetValue(propertyName);

            return null == token ? string.Empty : token.ToString();
        }
    }
}
