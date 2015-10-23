using System;
using System.Collections.Generic;
using System.Linq;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.AdvancedSearch.Replication.Bulk.Metamodel;
using NuClear.AdvancedSearch.Replication.Bulk.Processors;
using NuClear.CustomerIntelligence.Storage;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Storage.API.Readings;

using Identity = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace NuClear.AdvancedSearch.Replication.Bulk.Model
{
    public sealed class MassReplicationMetadataSource : MetadataSourceBase<MassReplicationMetadataKindIdentity>
    {
        private static readonly Storage Erm = new Storage("Erm", Schema.Erm);
        private static readonly Storage Facts = new Storage("Facts", Schema.Facts);
        private static readonly Storage CustomerIntelligence = new Storage("CustomerIntelligence", Schema.CustomerIntelligence);

        private static readonly Func<IMetadataElement, IQuery, DataConnection, IMassProcessor> FactProcessorFactory
            = (info, source, target) => CreateMassProcessor(typeof(FactMassProcessor<>), GetType(info), info, source, target);

        private static readonly Func<IMetadataFeature, IQuery, DataConnection, IMassProcessor> ValueObjectProcessorFactory
            = (info, source, target) => CreateMassProcessor(typeof(ValueObjectMassProcessor<>), GetType(info), info, source, target);

        private static readonly Func<IMetadataElement, IQuery, DataConnection, IMassProcessor> AggregateProcessorFactory
            = (info, source, target) => CreateMassProcessor(typeof(AggregateMassProcessor<>), GetType(info), info, source, target, ValueObjectProcessorFactory);

        private static readonly Func<IMetadataElement, IQuery, DataConnection, IMassProcessor> StatisticsProcessorFactory
            = (info, source, target) => CreateMassProcessor(typeof(StatisticsMassProcessor<>), GetType(info), info, source, target);


        private static readonly IReadOnlyDictionary<Uri, IMetadataElement> Elements =
            new MassReplicationMetadataElement[]
            {
                MassReplicationMetadataElement.Config
                    .Replication(Erm, Facts)
                    .CommandlineKey("-fact")
                    .ReplicationMetadataIdentity(Identity.Id.For<ReplicationMetadataIdentity>("Facts"))
                    .Factory(FactProcessorFactory),

                MassReplicationMetadataElement.Config
                    .Replication(Facts, CustomerIntelligence)
                    .CommandlineKey("-ci")
                    .ReplicationMetadataIdentity(Identity.Id.For<ReplicationMetadataIdentity>("Aggregates"))
                    .Factory(AggregateProcessorFactory),

                MassReplicationMetadataElement.Config
                    .Replication(Facts, CustomerIntelligence)
                    .CommandlineKey("-statistics")
                    .ReplicationMetadataIdentity(Identity.Id.For<StatisticsRecalculationMetadataIdentity>())
                    .Factory(StatisticsProcessorFactory)
                    .EssentialView("bit.firmcategory"),
            }.ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);

        private static IMassProcessor CreateMassProcessor(Type massProcessorGenericType, Type entityType, params object[] args)
        {
            return (IMassProcessor)Activator.CreateInstance(massProcessorGenericType.MakeGenericType(entityType), args);
        }

        private static Type GetType<T>(T element)
        {
            return element.GetType().GetGenericArguments().Single();
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata => Elements;
    }
}