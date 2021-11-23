using System;
using System.Text;

namespace Core
{
    public static class Base64Handler
    {
        public static string Encode(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string Decode(string text)
        {
            var bytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
