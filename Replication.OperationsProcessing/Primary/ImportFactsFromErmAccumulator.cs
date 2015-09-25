using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Model.Common.Entities;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Metadata.Model;
using NuClear.Replication.OperationsProcessing.Metadata.Model.Context;
using NuClear.Replication.OperationsProcessing.Metadata.Operations;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Telemetry;
using NuClear.Tracing.API;

namespace NuClear.Replication.OperationsProcessing.Primary
{
    /// <summary>
    /// Стратегия выполняет фильтрацию операций, приехавших в TUC, и преобразование этих операций в операции над фактами.
    /// </summary>
    public sealed class ImportFactsFromErmAccumulator : MessageProcessingContextAccumulatorBase<ImportFactsFromErmFlow, TrackedUseCase, OperationAggregatableMessage<FactOperation>>
    {
        private readonly ITracer _tracer;
        private readonly ITelemetryPublisher _telemetryPublisher;
		private readonly IEntityTypeMappingRegistry<FactsContext> _registry;

		public ImportFactsFromErmAccumulator(ITracer tracer, ITelemetryPublisher telemetryPublisher, IEntityTypeMappingRegistry<FactsContext> registry)
        {
            _tracer = tracer;
            _telemetryPublisher = telemetryPublisher;
			_registry = registry;
        }

        protected override OperationAggregatableMessage<FactOperation> Process(TrackedUseCase message)
        {
            _tracer.DebugFormat("Processing TUC {0}", message.Id);

            var receivedOperationCount = message.Operations.Sum(x => x.AffectedEntities.Changes.Sum(y => y.Value.Sum(z => z.Value.Count)));
            _telemetryPublisher.Publish<ErmReceivedOperationCountIdentity>(receivedOperationCount);

            var filteredOperations = Filter(message);
            var factOperations = Convert(filteredOperations).ToList();

            _telemetryPublisher.Publish<ErmEnqueuedOperationCountIdentity>(factOperations.Count);

            return new OperationAggregatableMessage<FactOperation>
            {
                       TargetFlow = MessageFlow,
                       Operations = factOperations,
                       OperationTime = message.Context.Finished.UtcDateTime,
                   };
        }

        private static IEnumerable<OperationDescriptor> Filter(TrackedUseCase message)
        {
            var operations = (IEnumerable<OperationDescriptor>)message.Operations;

            var disallowedIds = new HashSet<Guid>();
            var disallowedOperations = operations.Where(x => OperationIdentityMetadata.DisallowedOperationIdentities.Contains(x.OperationIdentity));
            foreach (var disallowedOperation in disallowedOperations)
            {
                disallowedIds.Add(disallowedOperation.Id);
                disallowedIds.UnionWith(message.GetNestedOperations(disallowedOperation.Id).Select(x => x.Id));
            }

            if (disallowedIds.Any())
            {
                operations = operations.Where(x => !disallowedIds.Contains(x.Id));
            }

            return operations;
        }

	    private IEnumerable<FactOperation> Convert(IEnumerable<OperationDescriptor> operations)
	    {
		    var factOperations = operations
			    .SelectMany(x =>
			                {

				                var tuples = x.AffectedEntities.Changes
				                              .Select(y =>
				                                      {
					                                      var mappedKey = y.Key.MapErmToFacts();

					                                      Type entityType;
					                                      var parsed = _registry.TryGetEntityType(mappedKey, out entityType);
					                                      return Tuple.Create(parsed, entityType, y);
				                                      })
				                              .Where(y => y.Item1).ToList();

				                if (tuples.Any())
				                {
					                if (!OperationIdentityMetadata.AllowedOperationIdentities.Contains(x.OperationIdentity))
					                {
						                var entitySet = new EntitySet(tuples.Select(y => y.Item3.Key).Distinct().ToArray());
						                _tracer.WarnFormat("Received well-known entities '{0}' frow unknown ERM operation '{1}'", entitySet, x.OperationIdentity);
					                }
				                }

				                return tuples;
			                })
			    .GroupBy(x => x.Item2, x => x.Item3.Value.Keys)
			    .SelectMany(x => x.SelectMany(y => y).Distinct().Select(y => new FactOperation(x.Key, y)));

		    return factOperations;
	    }
    }
}
