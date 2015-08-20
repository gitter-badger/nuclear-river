using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates;
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
        private readonly IDataChangesApplierFactory _dataChangesApplierFactory;

        public CustomerIntelligenceTransformation(IQuery query, IDataChangesApplierFactory dataChangesApplierFactory)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (dataChangesApplierFactory == null)
            {
                throw new ArgumentNullException("dataChangesApplierFactory");
            }

            _query = query;
            _dataChangesApplierFactory = dataChangesApplierFactory;
        }


        public void Transform(IEnumerable<AggregateOperation> operations)
        {
            using (Probe.Create("ETL2 Transforming"))
            {
                var slices = operations.GroupBy(x => new { Operation = x.GetType(), x.AggregateType })
                                       .OrderByDescending(x => x.Key.Operation, new AggregateOperationPriorityComparer());

                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                          new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                {
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

                        transaction.Complete();
                    }
                }
            }
        }

        private void InitializeAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            var changesDetector = new DataChangesDetector(aggregateInfo, _query);
            var mergeResult = changesDetector.DetectChanges(AggregateChangesDetectionMapSpec, aggregateIds);

            var aggregatesToCreateIds = aggregateIds.Where(x => mergeResult.Difference.Contains(x)).ToArray();
            var aggregatesToCreate = aggregateInfo.MapToSourceSpecProvider(aggregatesToCreateIds).Map(_query);

            var changesApplier = _dataChangesApplierFactory.Create(aggregateInfo.Type);
            changesApplier.Create(aggregatesToCreate);

            ApplyChangesToValueObjects(aggregateInfo.ValueObjects, aggregateIds);
        }

        private void RecalculateAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            ApplyChangesToValueObjects(aggregateInfo.ValueObjects, aggregateIds);

            var aggregateChangesDetector = new DataChangesDetector(aggregateInfo, _query);
            var mergeResult = aggregateChangesDetector.DetectChanges(AggregateChangesDetectionMapSpec, aggregateIds);

            var aggregatesToCreateIds = aggregateIds.Where(x => mergeResult.Difference.Contains(x)).ToArray();
            var aggregatesToUpdateIds = aggregateIds.Where(x => mergeResult.Intersection.Contains(x)).ToArray();
            var aggregatesToDeleteIds = aggregateIds.Where(x => mergeResult.Complement.Contains(x)).ToArray();

            var aggregatesToCreate = aggregateInfo.MapToSourceSpecProvider(aggregatesToCreateIds).Map(_query);
            var aggregatesToUpdate = aggregateInfo.MapToSourceSpecProvider(aggregatesToUpdateIds).Map(_query);
            var aggregatesToDelete = aggregateInfo.MapToTargetSpecProvider(aggregatesToDeleteIds).Map(_query);

            var changesApplier = _dataChangesApplierFactory.Create(aggregateInfo.Type);
            changesApplier.Delete(aggregatesToDelete);
            changesApplier.Create(aggregatesToCreate);
            changesApplier.Update(aggregatesToUpdate);
        }

        private void DestroyAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            ApplyChangesToValueObjects(aggregateInfo.ValueObjects, aggregateIds);

            var aggregateChangesDetector = new DataChangesDetector(aggregateInfo, _query);
            var mergeResult = aggregateChangesDetector.DetectChanges(AggregateChangesDetectionMapSpec, aggregateIds);

            var aggregatesToDeleteIds = aggregateIds.Where(x => mergeResult.Difference.Contains(x)).ToArray();
            var aggregatesToDelete = aggregateInfo.MapToTargetSpecProvider(aggregatesToDeleteIds).Map(_query).Cast<IIdentifiable>().ToArray();

            var changesApplier = _dataChangesApplierFactory.Create(aggregateInfo.Type);
            changesApplier.Delete(aggregatesToDelete);
        }

        private void ApplyChangesToValueObjects(IEnumerable<IMetadataInfo> valueObjectInfos, IReadOnlyCollection<long> aggregateIds)
        {
            foreach (var valueObjectInfo in valueObjectInfos)
            {
                var changesDetector = new DataChangesDetector(valueObjectInfo, _query);
                var mergeResult = changesDetector.DetectChanges(ValueObjectsChangesDetectionMapSpec, aggregateIds);

                var changesApplier = _dataChangesApplierFactory.Create(valueObjectInfo.Type);
                
                changesApplier.Delete(mergeResult.Complement);
                changesApplier.Create(mergeResult.Difference);
                changesApplier.Update(mergeResult.Intersection);
            }
        }
    }
}

