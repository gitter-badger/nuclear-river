﻿using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Metamodeling.Provider;
using NuClear.OperationsProcessing.API.Metadata;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

namespace NuClear.Replication.EntryPoint.Factories.Messaging.Receiver
{
    public sealed class FinalReceiverResolveStrategy : MessageFlowReceiverResolveStrategyBase
    {
        public FinalReceiverResolveStrategy(IMetadataProvider metadataProvider)
            : base(metadataProvider, typeof(SqlStoreReceiverTelemetryDecorator), PerformedOperations.IsPerformedOperationsFinalSource)
        {
        }
    }
}