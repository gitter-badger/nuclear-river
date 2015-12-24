using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Transports.File;
using NuClear.ValidationRules.OperationsProcessing.Identities.Flows;

namespace NuClear.Replication.EntryPoint.Factories.Messaging.Receiver
{
    // todo: как насчёт того, чтобы имя потока задавать в метаданных контекста? избавиться от ссылки на контекст хочется.
    public sealed class FileReceiverResolveStrategy : MessageFlowReceiverResolveStrategyBase
    {
        public FileReceiverResolveStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(FileReceiver), metadata => metadata.IsPerformedOperationsPrimarySource() && Equals(metadata.MessageFlow, ImportFactsFromOrderValidationConfigFlow.Instance))
        {
        }
    }
}