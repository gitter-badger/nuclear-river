using System;

using LinqToDB;
using LinqToDB.Data;

using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Replication.Bulk.Processors
{
    internal sealed class ProcessorHelper<T>
        where T : class
    {
        private readonly IQuery _source;
        private readonly DataConnection _target;
        private readonly FindSpecification<T> _spec;

        public ProcessorHelper(IQuery source, DataConnection target)
        {
            _source = source;
            _target = target;
            _spec = new FindSpecification<T>(x => true);
        }

        public void Process(MapToObjectsSpecProvider<T, T> mapSpecificationProviderForSource)
        {
            try
            {
                var sourceQueryable = mapSpecificationProviderForSource.Invoke(_spec).Map(_source);
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