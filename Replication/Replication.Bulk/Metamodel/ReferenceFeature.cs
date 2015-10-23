using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public class ReferenceFeature : IUniqueMetadataFeature
    {
        public ReferenceFeature(Uri reference)
        {
            Reference = reference;
        }

        public Uri Reference { get; }
    }
}