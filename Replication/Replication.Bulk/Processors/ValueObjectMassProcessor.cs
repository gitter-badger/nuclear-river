using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Features;
using NuClear.Storage.API.Readings;

namespace NuClear.AdvancedSearch.Replication.Bulk.Processors
{
	public class ValueObjectMassProcessor<T> : IMassProcessor 
		where T : class
	{
		private readonly ValueObjectFeature<T> _info;
		private readonly ProcessorHelper<T> _processorHelper;

		public ValueObjectMassProcessor(ValueObjectFeature<T> info, IQuery query, DataConnection connection)
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