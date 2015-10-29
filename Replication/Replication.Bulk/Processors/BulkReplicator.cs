using System;
using System.Collections.Generic;

using LinqToDB;
using LinqToDB.Data;

using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Replication.Bulk.Processors
{
	public sealed class BulkReplicator<T> : IBulkReplicator where T : class
	{
		private readonly IQuery _source;
		private readonly DataConnection _target;
	    private readonly MapSpecification<IQuery, IEnumerable<T>> _mapSpecification;

	    public BulkReplicator(IQuery source, DataConnection target, MapSpecification<IQuery, IEnumerable<T>> mapSpecification)
		{
			_source = source;
			_target = target;
	        _mapSpecification = mapSpecification;
		}

		public void Replicate()
		{
		    try
		    {
                var sourceQueryable = _mapSpecification.Map(_source);
                var options = new BulkCopyOptions { BulkCopyTimeout = 300 };
                _target.GetTable<T>().Delete();
                _target.BulkCopy(options, sourceQueryable);
            }
            catch (Exception ex)
		    {
		        throw new Exception($"Can not process entity type {typeof(T).Name}", ex);
		    }
		}
	}
}