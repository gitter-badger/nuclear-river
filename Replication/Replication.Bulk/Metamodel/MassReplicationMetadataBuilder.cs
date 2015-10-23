using System;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.Bulk.Processors;
using NuClear.Metamodeling.Elements;
using NuClear.Storage.API.Readings;

namespace NuClear.AdvancedSearch.Replication.Bulk.Metamodel
{
    public sealed class MassReplicationMetadataBuilder : MetadataElementBuilder<MassReplicationMetadataBuilder, MassReplicationMetadataElement>
    {
        public MassReplicationMetadataBuilder Replication(Storage source, Storage target)
        {
            AddFeatures(new DirectionFeature(source, target));
            return this;
        }

        public MassReplicationMetadataBuilder ReplicationMetadataIdentity(Uri reference)
        {
            AddFeatures(new ReferenceFeature(reference));
            return this;
        }

        public MassReplicationMetadataBuilder CommandlineKey(string key)
        {
            AddFeatures(new CommandLineFeature(key));
            return this;
        }

        public MassReplicationMetadataBuilder Factory(Func<IMetadataElement, IQuery, DataConnection, IMassProcessor> factory)
        {
            AddFeatures(new FactoryFeature(factory));
            return this;
        }

        public MassReplicationMetadataBuilder EssentialView(string viewName)
        {
            AddFeatures(new EssentialViewFeature(viewName));
            return this;
        }

        protected override MassReplicationMetadataElement Create()
        {
            return new MassReplicationMetadataElement(Features);
        }
    }
}