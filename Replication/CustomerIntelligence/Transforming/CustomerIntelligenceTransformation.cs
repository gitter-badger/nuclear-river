using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.API.Transforming;
using NuClear.AdvancedSearch.Replication.API.Transforming.Aggregates;
using NuClear.Telemetry.Probing;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    public sealed class CustomerIntelligenceTransformation
    {
        private readonly IMetadataSource<IAggregateInfo> _metadataSource;
        private readonly IAggregateProcessorFactory _aggregateProcessorFactory;
        private readonly IValueObjectProcessorFactory _valueObjectProcessorFactory;

        public CustomerIntelligenceTransformation(IMetadataSource<IAggregateInfo> metadataSource,
                                                  IAggregateProcessorFactory aggregateProcessorFactory,
                                                  IValueObjectProcessorFactory valueObjectProcessorFactory)
        {
            if (metadataSource == null)
            {
                throw new ArgumentNullException("metadataSource");
            }

            if (aggregateProcessorFactory == null)
            {
                throw new ArgumentNullException("aggregateProcessorFactory");
            }

            if (valueObjectProcessorFactory == null)
            {
                throw new ArgumentNullException("valueObjectProcessorFactory");
            }

            _metadataSource = metadataSource;
            _aggregateProcessorFactory = aggregateProcessorFactory;
            _valueObjectProcessorFactory = valueObjectProcessorFactory;
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
            var processor = _aggregateProcessorFactory.Create(aggregateInfo);
            processor.Initialize(aggregateIds);

            ApplyChangesToValueObjects(aggregateInfo.ValueObjects, aggregateIds);
        }

        private void RecalculateAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            ApplyChangesToValueObjects(aggregateInfo.ValueObjects, aggregateIds);

            var processor = _aggregateProcessorFactory.Create(aggregateInfo);
            processor.Recalculate(aggregateIds);
        }

        private void DestroyAggregate(IAggregateInfo aggregateInfo, IReadOnlyCollection<long> aggregateIds)
        {
            ApplyChangesToValueObjects(aggregateInfo.ValueObjects, aggregateIds);

            var processor = _aggregateProcessorFactory.Create(aggregateInfo);
            processor.Destroy(aggregateIds);
        }

        private void ApplyChangesToValueObjects(IEnumerable<IValueObjectInfo> valueObjectInfos, IReadOnlyCollection<long> aggregateIds)
        {
            foreach (var valueObjectInfo in valueObjectInfos)
            {
                var transformation = _valueObjectProcessorFactory.Create(valueObjectInfo);
                transformation.ApplyChanges(aggregateIds);
            }
        }
    }
}

