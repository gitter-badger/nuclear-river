using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class CustomerIntelligenceTransformation
    {
        private readonly ICustomerIntelligenceContext _source;
        private readonly ICustomerIntelligenceContext _target;
        private readonly IDataMapper _mapper;
        private readonly ITransactionManager _transactionManager;

        public CustomerIntelligenceTransformation(ICustomerIntelligenceContext source, ICustomerIntelligenceContext target, IDataMapper mapper, ITransactionManager transactionManager)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            _source = source;
            _target = target;
            _mapper = mapper;
            _transactionManager = transactionManager;
        }


        public void Transform(IEnumerable<AggregateOperation> operations)
        {
            using (Probe.Create("ETL2 Transforming"))
            {
                _transactionManager.WithinTransaction(() => DoTransform(operations));
            }
        }

        private void DoTransform(IEnumerable<AggregateOperation> operations)
        {
            var slices = operations.GroupBy(x => new { Operation = x.GetType(), x.AggregateType })
                                   .OrderByDescending(x => x.Key.Operation, new AggregateOperationPriorityComparer());

            foreach (var slice in slices)
            {
                var operation = slice.Key.Operation;
                var aggregateType = slice.Key.AggregateType;
                var aggregateIds = slice.Select(x => x.AggregateId).Distinct().ToArray();

                AggregateInfo aggregateInfo;
                if (!Aggregates.TryGetValue(aggregateType, out aggregateInfo))
                {
                    throw new NotSupportedException(string.Format("The '{0}' aggregate not supported.", aggregateType));
                }

                using (Probe.Create("ETL2 Transforming", aggregateInfo.AggregateType.Name))
                {
                    if (operation == typeof(InitializeAggregate))
                    {
                        InitializeAggregate(aggregateInfo, aggregateIds);
                    }

                    if (operation == typeof(RecalculateAggregate))
                    {
                        RecalculateAggregate(aggregateInfo, aggregateIds);
                    }

                    if (operation == typeof(DestroyAggregate))
                    {
                        DestroyAggregate(aggregateInfo, aggregateIds);
                    }
                }
            }
        }

        private void InitializeAggregate(AggregateInfo aggregateInfo, long[] ids)
        {
            _mapper.InsertAll(aggregateInfo.Query(_source, ids));

            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                _mapper.InsertAll(valueObjectInfo.Query(_source, ids));
            }
        }

        private void RecalculateAggregate(AggregateInfo aggregateInfo, long[] ids)
        {
            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                var query = valueObjectInfo.Query;
                var result = MergeTool.Merge(query(_source, ids), query(_target, ids));

                var elementsToInsert = result.Difference;
                var elementsToUpdate = result.Intersection;
                var elementsToDelete = result.Complement;

                _mapper.InsertAll(elementsToInsert.AsQueryable());
                _mapper.UpdateAll(elementsToUpdate.AsQueryable());
                _mapper.DeleteAll(elementsToDelete.AsQueryable());
            }

            _mapper.UpdateAll(aggregateInfo.Query(_source, ids));

            foreach (var entityInfo in aggregateInfo.Entities)
            {
                var query = entityInfo.Query;
                var result = MergeTool.Merge(query(_source, ids), query(_target, ids));

                var elementsToInsert = result.Difference;
                var elementsToUpdate = result.Intersection;
                var elementsToDelete = result.Complement;

                _mapper.InsertAll(elementsToInsert.AsQueryable());
                _mapper.UpdateAll(elementsToUpdate.AsQueryable());
                _mapper.DeleteAll(elementsToDelete.AsQueryable());
            }
        }

        private void DestroyAggregate(AggregateInfo aggregateInfo, long[] ids)
        {
            foreach (var entityInfo in aggregateInfo.Entities)
            {
                _mapper.DeleteAll(entityInfo.Query(_target, ids));
            }

            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                _mapper.DeleteAll(valueObjectInfo.Query(_target, ids));
            }

            _mapper.DeleteAll(aggregateInfo.Query(_target, ids));
        }
    }
}

