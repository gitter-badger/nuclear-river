using System;

namespace NuClear.AdvancedSearch.Common.Metadata.Elements
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