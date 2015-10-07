using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.Storage.API.Readings;

namespace NuClear.AdvancedSearch.Replication.Bulk.Processors
{
	public class StatisticsMassProcessor<T> : IMassProcessor 
		where T : class
	{
		private readonly StatisticsRecalculationMetadata<T> _info;
		private readonly ProcessorHelper<T> _processorHelper;

		public StatisticsMassProcessor(StatisticsRecalculationMetadata<T> info, IQuery query, DataConnection connection)
		{
			_info = info;
			_processorHelper = new ProcessorHelper<T>(query, connection);
		}

		public void Process()
		{
			_processorHelper.Process(_info.MapSpecificationProviderForSource);
		}
	}
}