using System;
using System.Linq;

using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Features;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

namespace NuClear.AdvancedSearch.Replication.Bulk.Processors
{
	public class AggregateMassProcessor<T> : IMassProcessor 
		where T : class, IIdentifiable
	{
		private readonly AggregateMetadata<T> _info;
		private readonly ProcessorHelper<T> _processorHelper;
		private readonly IMassProcessor[] _subprocessors;

		public AggregateMassProcessor(AggregateMetadata<T> info, IQuery query, DataConnection connection, Func<IValueObjectFeature, IQuery, DataConnection, IMassProcessor> valueObjectProcessorfactory)
		{
			_info = info;
			_processorHelper = new ProcessorHelper<T>(query, connection);
			_subprocessors = info.Features.OfType<IValueObjectFeature>().Select(valueObjectInfo => valueObjectProcessorfactory.Invoke(valueObjectInfo, query, connection)).ToArray();
		}

		public void Process()
		{
			_processorHelper.Process(_info.MapSpecificationProviderForSource);

			foreach (var processor in _subprocessors)
			{
				processor.Process();
			}
		}
	}
}