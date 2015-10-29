using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public sealed class BulkReplicationMetadataElement : MetadataElement<BulkReplicationMetadataElement, BulkReplicationMetadataBuilder>
    {
        private IMetadataElementIdentity _identity;

        public BulkReplicationMetadataElement(IEnumerable<IMetadataFeature> features) 
            : base(features)
        {
            var commandLine = GetFeature<CommandLineFeature>();
            _identity = Metadata.Id.For<BulkReplicationMetadataKindIdentity>(commandLine.Key).Build().AsIdentity();
        }

        public override IMetadataElementIdentity Identity => _identity;

        
        public string CommandLineKey => GetFeature<CommandLineFeature>().Key;

        public IEnumerable<string> EssentialViews => Features.OfType<EssentialViewFeature>().Select(feature => feature.ViewName);

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity) => _identity = actualMetadataElementIdentity;

        private T GetFeature<T>()
        {
            return Features.OfType<T>().Single();
        }
    }
}
