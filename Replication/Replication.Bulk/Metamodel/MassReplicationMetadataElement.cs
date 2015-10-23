using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.Bulk.Processors;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Storage.API.Readings;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public sealed class MassReplicationMetadataElement : MetadataElement<MassReplicationMetadataElement, MassReplicationMetadataBuilder>
    {
        private IMetadataElementIdentity _identity;

        public MassReplicationMetadataElement(IEnumerable<IMetadataFeature> features) 
            : base(features)
        {
            var commandLine = GetFeature<CommandLineFeature>();
            _identity = Metadata.Id.For<MassReplicationMetadataKindIdentity>(commandLine.Key).Build().AsIdentity();
        }

        public override IMetadataElementIdentity Identity 
            => _identity;

        public Storage Source
            => GetFeature<DirectionFeature>().Source;

        public Storage Target
            => GetFeature<DirectionFeature>().Target;

        public Uri MetadataReference
            => GetFeature<ReferenceFeature>().Reference;

        public Func<IMetadataElement, IQuery, DataConnection, IMassProcessor> Factory
            => GetFeature<FactoryFeature>().Factory;

        public string CommandLineKey
            => GetFeature<CommandLineFeature>().Key;

        public IEnumerable<string> EssentialViews
            => Features.OfType<EssentialViewFeature>().Select(feature => feature.ViewName);

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity) 
            => _identity = actualMetadataElementIdentity;

        private T GetFeature<T>()
        {
            return Features.OfType<T>().Single();
        }
    }
}
