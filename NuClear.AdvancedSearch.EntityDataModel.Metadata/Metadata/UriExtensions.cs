using System;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
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