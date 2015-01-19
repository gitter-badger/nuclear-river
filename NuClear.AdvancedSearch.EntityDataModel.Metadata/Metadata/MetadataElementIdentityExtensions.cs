using System;
using System.Linq;

using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    internal static class MetadataElementIdentityExtensions
    {
        public static string ResolveFullName(this IMetadataElementIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            if (identity.Id == null)
            {
                throw new InvalidOperationException("The id was not specified.");
            }

            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.Id.GetComponents(UriComponents.Path, UriFormat.Unescaped).Replace('/', '.');
        }

        public static string ResolveName(this IMetadataElementIdentity identity)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            if (identity.Id == null)
            {
                throw new InvalidOperationException("The id was not specified.");
            }

            // TODO {s.pomadin, 16.12.2014}: provide a better solution
            return identity.Id.GetComponents(UriComponents.Path, UriFormat.Unescaped).Split('/').LastOrDefault();
        }
    }
}