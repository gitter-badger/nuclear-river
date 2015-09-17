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

        public CustomerIntelligenceTransformation(IMetadataSource<IAggregateInfo> metadataSource,
                                                  IAggregateProcessorFactory aggregateProcessorFactory)
        {
            if (metadataSource == null)
            {
                throw new ArgumentNullException("metadataSource");
            }

            if (aggregateProcessorFactory == null)
            {
                throw new ArgumentNullException("aggregateProcessorFactory");
            }

            _metadataSource = metadataSource;
            _aggregateProcessorFactory = aggregateProcessorFactory;
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
                    var aggregateIds = slice.Select(x => x.AggregateId).Distinct().ToArray();

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
							var processor = _aggregateProcessorFactory.Create(aggregateInfo);

							if (operation == typeof(InitializeAggregate))
                            {
								processor.Initialize(aggregateIds);
								continue;
                            }

                            if (operation == typeof(RecalculateAggregate))
                            {
								processor.Recalculate(aggregateIds);
								continue;
                            }

                            if (operation == typeof(DestroyAggregate))
                            {
								processor.Destroy(aggregateIds);
								continue;
                            }
                        }

                        transaction.Complete();
                    }
                }
            }
        }
    }
}

