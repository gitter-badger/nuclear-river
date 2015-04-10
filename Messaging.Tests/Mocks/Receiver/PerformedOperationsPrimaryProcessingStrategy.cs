using System.Collections.Generic;
using System.IO;

using NuClear.AdvancedSearch.Messaging.ServiceBus;
using NuClear.AdvancedSearch.Messaging.Tests.Properties;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Receivers;
using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.OperationsTracking.API.UseCases;

namespace NuClear.AdvancedSearch.Messaging.Tests.Mocks.Receiver
{
    public sealed class PerformedOperationsPrimaryProcessingStrategy : MessageFlowReceiverResolveStrategyBase
    {
        public PerformedOperationsPrimaryProcessingStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(ServiceBusOperationsReceiver), PerformedOperations.IsPerformedOperationsPrimarySource)
        {

        }

        private sealed class ServiceBusOperationsReceiver : MessageReceiverBase<TrackedUseCase, IPerformedOperationsReceiverSettings>
        {
            private readonly ITrackedUseCaseParser _parser;

            public ServiceBusOperationsReceiver(MessageFlowMetadata sourceFlowMetadata, IPerformedOperationsReceiverSettings messageReceiverSettings, ITrackedUseCaseParser parser)
                : base(sourceFlowMetadata, messageReceiverSettings)
            {
                _parser = parser;
            }

            protected override IReadOnlyList<TrackedUseCase> Peek()
            {
                var stream = new MemoryStream(Resources.UpdateFirm);
                var useCase = _parser.Parse(stream);

                return new[] { useCase };
            }

            protected override void Complete(IEnumerable<TrackedUseCase> successfullyProcessedMessages, IEnumerable<TrackedUseCase> failedProcessedMessages)
            {

            }
        }
    }
}