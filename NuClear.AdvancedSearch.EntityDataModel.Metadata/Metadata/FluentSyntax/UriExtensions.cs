using System;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    internal static class UriExtensions
    {
        public static Uri AsRelativeUri(this string uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException("uri");
            }

            return new Uri(uri, UriKind.Relative);
        }
    }
}
