using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed partial class AdvancedSearchMetadataSource : MetadataSourceBase<AdvancedSearchIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public AdvancedSearchMetadataSource()
        {
            _metadata = new Dictionary<Uri, IMetadataElement> { { CustomerIntelligenceContext.Identity.Id, CustomerIntelligenceContext } };
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