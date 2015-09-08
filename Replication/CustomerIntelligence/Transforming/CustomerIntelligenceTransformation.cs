using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates;
using NuClear.Storage.Readings;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class CustomerIntelligenceTransformation
    {
        private readonly IQuery _query;
        private readonly IDataChangesApplierFactory _dataChangesApplierFactory;
        private readonly IMetadataSource<IAggregateInfo> _metadataSource;
        private readonly IAggregateProcessorFactory _aggregateProcessorFactory;
        private readonly IValueObjectProcessorFactory _valueObjectProcessorFactory;

        public CustomerIntelligenceTransformation(IQuery query, IDataChangesApplierFactory dataChangesApplierFactory, IMetadataSource<IAggregateInfo> metadataSource)
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
            _metadataSource = metadataSource;
            _aggregateProcessorFactory = new AggregateProcessorFactory();
            _valueObjectProcessorFactory = new ValueObjectProcessorFactory();
        }


        public void Transform(IEnumerable<AggregateOperation> operations)
        {
            using (Probe.Create("ETL2 Transforming"))
            {
                var slices = operations.GroupBy(x => new { Operation = x.GetType(), x.AggregateType })
                                       .OrderByDescending(x => x.Key.Operation, new AggregateOperationPriorityComparer());

                foreach (var slice in slices)
                {
                    var operation = slice.Key.Operation;
                    var aggregateType = slice.Key.AggregateType;
                    var aggregateIds = slice.Select(x => x.AggregateId).Distinct().ToList();

                    IAggregateInfo aggregateInfo;
                    if (!_metadataSource.Metadata.TryGetValue(aggregateType, out aggregateInfo))
                    {
                        throw new NotSupportedException(string.Format("The '{0}' aggregate not supported.", aggregateType));
                    }

                    using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                                                                  new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TimeSpan.Zero }))
                    {
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
            var changesApplier = _dataChangesApplierFactory.Create(aggregateInfo.Type);
            var processor = _aggregateProcessorFactory.Create(aggregateInfo);
            processor.Initialize(_query, changesApplier, aggregateIds);

            ApplyChangesToValueObjects(aggregateInfo.ValueObjects, aggregateIds);
        }

        private void RecalculateAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            ApplyChangesToValueObjects(aggregateInfo.ValueObjects, aggregateIds);

            var changesApplier = _dataChangesApplierFactory.Create(aggregateInfo.Type);
            var processor = _aggregateProcessorFactory.Create(aggregateInfo);
            processor.Recalculate(_query, changesApplier, aggregateIds);
        }

        private void DestroyAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            ApplyChangesToValueObjects(aggregateInfo.ValueObjects, aggregateIds);

            var changesApplier = _dataChangesApplierFactory.Create(aggregateInfo.Type);
            var processor = _aggregateProcessorFactory.Create(aggregateInfo);
            processor.Destroy(_query, changesApplier, aggregateIds);
        }

        private void ApplyChangesToValueObjects(IEnumerable<IValueObjectInfo> valueObjectInfos, IReadOnlyCollection<long> aggregateIds)
        {
            foreach (var valueObjectInfo in valueObjectInfos)
            {
                var changesApplier = _dataChangesApplierFactory.Create(valueObjectInfo.Type);
                var transformation = _valueObjectProcessorFactory.Create(valueObjectInfo);
                transformation.ApplyChanges(_query, changesApplier, aggregateIds);
            }
        }
    }
}

