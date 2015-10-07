using System;
using System.Linq;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Replication.Bulk.Processors;
using NuClear.CustomerIntelligence.Domain;
using NuClear.CustomerIntelligence.Storage;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Storage.API.Readings;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
	public sealed class Context
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

		public static readonly MassReplicationContext ErmToFacts = new MassReplicationContext(
			Erm,
			Facts,
            new FactsReplicationMetadataSource().Metadata.Values.Cast<HierarchyMetadata>().Single(),
			FactProcessorFactory);

		public static readonly MassReplicationContext FactsToCustomerIntelligence = new MassReplicationContext(
			Facts,
			CustomerIntelligence,
            new AggregateConstructionMetadataSource().Metadata.Values.Cast<HierarchyMetadata>().Single(),
			AggregateProcessorFactory);

		public static readonly MassReplicationContext FactsToCustomerIntelligenceStatistics = new MassReplicationContext(
			Facts,
			CustomerIntelligence,
            new StatisticsRecalculationMetadataSource().Metadata.Values.Cast<HierarchyMetadata>().Single(),
			StatisticsProcessorFactory);

		private static IMassProcessor CreateMassProcessor(Type massProcessorGenericType, Type entityType, params object[] args)
		{
			return (IMassProcessor)Activator.CreateInstance(massProcessorGenericType.MakeGenericType(entityType), args);
		}

	    private static Type GetType(object x)
	    {
	        return x.GetType().GetGenericArguments().Single();
	    }
	}
}