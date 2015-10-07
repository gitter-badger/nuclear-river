using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Storage.API.Readings;

namespace NuClear.AdvancedSearch.Replication.Bulk.Processors
{
	public class FactMassProcessor<T> : IMassProcessor 
        where T : class, IIdentifiable
	{
		private readonly FactMetadata<T> _info;
		private readonly ProcessorHelper<T> _processorHelper;

	    public FactMassProcessor(FactMetadata<T> info, IQuery query, DataConnection connection)
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