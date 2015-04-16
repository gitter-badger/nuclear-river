using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.DI.Factories.Unity.Transformers.Resolvers;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Transformer
{
    public sealed class PrimaryMessageTransformerResolveStrategy : IMessageTransformerResolveStrategy
    {
        private readonly ISet<IMetadataElementIdentity> _appropriateProcessingFlowsRegistrar;

        public PrimaryMessageTransformerResolveStrategy(IMetadataProvider metadataProvider)
        {
            _appropriateProcessingFlowsRegistrar =
                new HashSet<IMetadataElementIdentity>(metadataProvider.GetConcreteMetadataOfKind<MetadataMessageFlowsIdentity, MessageFlowMetadata>()
                                                          .Where(PerformedOperations.IsPerformedOperationsPrimarySource)
                                                          .Select(metadata => metadata.Identity));
        }

        public bool TryGetAppropriateTransformer(MessageFlowMetadata messageFlowMetadata, out Type resolvedFlowReceiverType)
        {
            if (!_appropriateProcessingFlowsRegistrar.Contains(messageFlowMetadata.Identity))
            {
                resolvedFlowReceiverType = null;
                return false;
            }

            resolvedFlowReceiverType = typeof(BinaryEntireBrokeredMessage2TrackedUseCaseTransformer);
            return true;
        }
    }
}
