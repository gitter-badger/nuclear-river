using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider.Sources;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed partial class AdvancedSearchMetadataSource : MetadataSourceBase<AdvancedSearchIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public AdvancedSearchMetadataSource()
        {
            HierarchyMetadata root = HierarchyMetadata.Config
                .Id.Is(IdBuilder.For<AdvancedSearchIdentity>())
                .Childs(_customerIntelligence);
            _metadata = new Dictionary<Uri, IMetadataElement> { { root.Identity.Id, root } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get
            {
                return _metadata;
            }
        }
    }
}