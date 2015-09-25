using System;

namespace NuClear.Querying.Metadata
{
    internal static class UriExtensions
    {
        public static Uri AsUri(this string uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            return new Uri(uri, UriKind.RelativeOrAbsolute);
        }
    }
}