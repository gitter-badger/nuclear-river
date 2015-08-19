using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.DataMappers;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class CustomerIntelligenceTransformation
    {
        private static readonly MapSpecification<IEnumerable, IEnumerable<long>> AggregateChangesDetectionMapSpec = 
            new MapSpecification<IEnumerable, IEnumerable<long>>(x => x.Cast<IIdentifiable>().Select(y => y.Id));

        private static readonly MapSpecification<IEnumerable, IEnumerable<IObject>> ValueObjectsChangesDetectionMapSpec =
            new MapSpecification<IEnumerable, IEnumerable<IObject>>(x => x.Cast<IObject>());

        private readonly IQuery _query;
        private readonly IDataMapper _mapper;
        private readonly ITransactionManager _transactionManager;
        
        public CustomerIntelligenceTransformation(IQuery query, IDataMapper mapper, ITransactionManager transactionManager)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            _query = query;
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
                var aggregateIds = slice.Select(x => x.AggregateId).Distinct().ToList();

                IAggregateInfo aggregateInfo;
                if (!Aggregates.TryGetValue(aggregateType, out aggregateInfo))
                {
                    throw new NotSupportedException(string.Format("The '{0}' aggregate not supported.", aggregateType));
                }

                using (Probe.Create("ETL2 Transforming", aggregateType.Name))
                {
                    if (operation == typeof(InitializeAggregate))
                    {
                        InitializeAggregate(aggregateInfo, aggregateIds);
                        continue;
                    }

                    if (operation == typeof(RecalculateAggregate))
                    {
                        RecalculateAggregate(aggregateInfo, aggregateIds);
                        continue;
                    }

                    if (operation == typeof(DestroyAggregate))
                    {
                        DestroyAggregate(aggregateInfo, aggregateIds);
                        continue;
                    }
                }
            }
        }

        private void InitializeAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            var aggregateChangesDetector = new DataChangesDetector(aggregateInfo, _query);
            var mergeResult = aggregateChangesDetector.DetectChanges(AggregateChangesDetectionMapSpec, aggregateIds);

            var sourceAggregates = aggregateInfo.MapToSourceSpecProvider(aggregateIds).Map(_query).Cast<IIdentifiable>().ToArray();
            var aggregateDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, aggregateInfo);
            aggregateDataMapper.Insert(sourceAggregates.Where(x => mergeResult.Difference.Contains(x.Id)));

            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                var valueObjectChangesDetector = new DataChangesDetector(valueObjectInfo, _query);
                var valueObjectMergeResult = valueObjectChangesDetector.DetectChanges(ValueObjectsChangesDetectionMapSpec, aggregateIds);

                var valueObjectDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, valueObjectInfo);
                valueObjectDataMapper.Insert(valueObjectMergeResult.Difference);
            }
        }

        private void RecalculateAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                var valueObjectChangesDetector = new DataChangesDetector(valueObjectInfo, _query);
                var valueObjectMergeResult = valueObjectChangesDetector.DetectChanges(ValueObjectsChangesDetectionMapSpec, aggregateIds);

                var valueObjectDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, valueObjectInfo);
                valueObjectDataMapper.Delete(valueObjectMergeResult.Complement);
                valueObjectDataMapper.Insert(valueObjectMergeResult.Difference);
                valueObjectDataMapper.Update(valueObjectMergeResult.Intersection);
            }

            var aggregateChangesDetector = new DataChangesDetector(aggregateInfo, _query);
            var mergeResult = aggregateChangesDetector.DetectChanges(AggregateChangesDetectionMapSpec, aggregateIds);

            var sourceAggregates = aggregateInfo.MapToSourceSpecProvider(aggregateIds).Map(_query).Cast<IIdentifiable>().ToArray();
            var targetAggregates = aggregateInfo.MapToTargetSpecProvider(aggregateIds).Map(_query).Cast<IIdentifiable>().ToArray();

            var aggregateDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, aggregateInfo);
            aggregateDataMapper.Delete(targetAggregates.Where(x => mergeResult.Complement.Contains(x.Id)));
            aggregateDataMapper.Insert(sourceAggregates.Where(x => mergeResult.Difference.Contains(x.Id)));
            aggregateDataMapper.Update(sourceAggregates.Where(x => mergeResult.Intersection.Contains(x.Id)));
        }

        private void DestroyAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                var valueObjectChangesDetector = new DataChangesDetector(valueObjectInfo, _query);
                var valueObjectMergeResult = valueObjectChangesDetector.DetectChanges(ValueObjectsChangesDetectionMapSpec, aggregateIds);
                
                var valueObjectDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, valueObjectInfo);
                valueObjectDataMapper.Delete(valueObjectMergeResult.Complement);
            }

            var aggregateChangesDetector = new DataChangesDetector(aggregateInfo, _query);
            var mergeResult = aggregateChangesDetector.DetectChanges(AggregateChangesDetectionMapSpec, aggregateIds);

            var targetAggregates = aggregateInfo.MapToTargetSpecProvider(aggregateIds).Map(_query).Cast<IIdentifiable>().ToArray();

            var aggregateDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, aggregateInfo);
            aggregateDataMapper.Delete(targetAggregates.Where(x => mergeResult.Complement.Contains(x.Id)));
        }
    }
}

