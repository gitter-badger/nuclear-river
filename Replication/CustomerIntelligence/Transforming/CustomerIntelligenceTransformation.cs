using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.DataMappers;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed partial class CustomerIntelligenceTransformation
    {
        private readonly IQuery _source;
        private readonly IQuery _target;
        private readonly IDataMapper _mapper;
        private readonly ITransactionManager _transactionManager;
        private readonly MapSpecification<IEnumerable, IEnumerable<IObject>> _valueObjectschangesDetectionMapSpec;

        public CustomerIntelligenceTransformation(IQuery source, IQuery target, IDataMapper mapper, ITransactionManager transactionManager)
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
            _valueObjectschangesDetectionMapSpec = new MapSpecification<IEnumerable, IEnumerable<IObject>>(x => x.Cast<IObject>());
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
            var sourceAggregates = aggregateInfo.MapToSourceSpecProvider(aggregateIds).Map(_source).Cast<IIdentifiable>().ToArray();
            
            var sourceAggregateIds = sourceAggregates.Select(x => x.Id);
            var targetAggregateIds = aggregateInfo.MapToTargetSpecProvider(aggregateIds).Map(_target).Cast<IIdentifiable>().Select(x => x.Id).ToArray();
            
            var mergeResult = MergeTool.Merge(sourceAggregateIds, targetAggregateIds);

            var aggregateDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, aggregateInfo);
            aggregateDataMapper.Insert(sourceAggregates.Where(x => mergeResult.Difference.Contains(x.Id)));

            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                var changesDetector = new SourceChangesDetector(valueObjectInfo, _source, _target);
                var valueObjectMergeResult = changesDetector.DetectChanges(_valueObjectschangesDetectionMapSpec, aggregateIds);

                var valueObjectDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, valueObjectInfo);
                valueObjectDataMapper.Insert(valueObjectMergeResult.Difference);
            }
        }

        private void RecalculateAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                var changesDetector = new SourceChangesDetector(valueObjectInfo, _source, _target);
                var valueObjectMergeResult = changesDetector.DetectChanges(_valueObjectschangesDetectionMapSpec, aggregateIds);

                var valueObjectDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, valueObjectInfo);
                valueObjectDataMapper.Delete(valueObjectMergeResult.Complement);
                valueObjectDataMapper.Insert(valueObjectMergeResult.Difference);
                valueObjectDataMapper.Update(valueObjectMergeResult.Intersection);
            }

            var sourceAggregates = aggregateInfo.MapToSourceSpecProvider(aggregateIds).Map(_source).Cast<IIdentifiable>().ToArray();
            var targetAggregates = aggregateInfo.MapToTargetSpecProvider(aggregateIds).Map(_target).Cast<IIdentifiable>().ToArray();

            var sourceAggregateIds = sourceAggregates.Select(x => x.Id);
            var targetAggregateIds = targetAggregates.Select(x => x.Id);

            var mergeResult = MergeTool.Merge(sourceAggregateIds, targetAggregateIds);

            var aggregateDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, aggregateInfo);
            aggregateDataMapper.Delete(targetAggregates.Where(x => mergeResult.Complement.Contains(x.Id)));
            aggregateDataMapper.Insert(sourceAggregates.Where(x => mergeResult.Difference.Contains(x.Id)));
            aggregateDataMapper.Update(sourceAggregates.Where(x => mergeResult.Intersection.Contains(x.Id)));
        }

        private void DestroyAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            foreach (var valueObjectInfo in aggregateInfo.ValueObjects)
            {
                var changesDetector = new SourceChangesDetector(valueObjectInfo, _source, _target);
                var valueObjectMergeResult = changesDetector.DetectChanges(_valueObjectschangesDetectionMapSpec, aggregateIds);
                
                var valueObjectDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, valueObjectInfo);
                valueObjectDataMapper.Delete(valueObjectMergeResult.Complement);
            }

            var targetAggregates = aggregateInfo.MapToTargetSpecProvider(aggregateIds).Map(_target).Cast<IIdentifiable>().ToArray();
            var sourceAggregateIds = aggregateInfo.MapToSourceSpecProvider(aggregateIds).Map(_source).Cast<IIdentifiable>().Select(x => x.Id).ToArray();
            var targetAggregateIds = targetAggregates.Select(x => x.Id);
            
            var mergeResult = MergeTool.Merge(sourceAggregateIds, targetAggregateIds);

            var aggregateDataMapper = DataMapperFactory.CreateTypedDataMapper(_mapper, aggregateInfo);
            aggregateDataMapper.Delete(targetAggregates.Where(x => mergeResult.Complement.Contains(x.Id)));
        }
    }
}

