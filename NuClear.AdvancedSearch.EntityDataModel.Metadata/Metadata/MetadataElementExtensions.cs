using System;

using NuClear.Metamodeling.Elements;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public static class MetadataElementExtensions
    {
        public static string ResolveFullName(this IMetadataElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return element.Identity.ResolveFullName();
        }

        public static string ResolveName(this IMetadataElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            return element.Identity.ResolveName();
        }

        public static string ResolveSchema(this IMetadataElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            var name = element.Identity.ResolveName();
            if (name != null)
            {
                var index = name.IndexOf('.');
                if (index >= 0)
                {
                    return name.Substring(0, index);
                }
            }
            return null;
        }
    }
}