using System.Collections.Generic;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Messaging.Metadata.Flows;
using NuClear.AdvancedSearch.Messaging.ServiceBus;
using NuClear.AdvancedSearch.Messaging.Tests.Mocks.Receiver;
using NuClear.DI.Unity.Config;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.Messaging.API.Processing.Actors.Transformers;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Messaging.API.Receivers;
using NuClear.Messaging.DI.Factories.Unity.Accumulators;
using NuClear.Messaging.DI.Factories.Unity.Common;
using NuClear.Messaging.DI.Factories.Unity.Handlers;
using NuClear.Messaging.DI.Factories.Unity.Processors;
using NuClear.Messaging.DI.Factories.Unity.Processors.Resolvers;
using NuClear.Messaging.DI.Factories.Unity.Receivers;
using NuClear.Messaging.DI.Factories.Unity.Receivers.Resolvers;
using NuClear.Messaging.DI.Factories.Unity.Stages;
using NuClear.Messaging.DI.Factories.Unity.Transformers;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Processors.Concrete;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.Tracing.API;

namespace NuClear.AdvancedSearch.Messaging.Tests.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(this IUnityContainer container, IReadOnlyList<byte[]> messageBytes)
        {
            var settings = new PerformedOperationsPrimaryFlowProcessorSettings
            {
                AppropriatedStages = new[]
                {
                    MessageProcessingStage.Transforming,
                    MessageProcessingStage.Processing,
                    MessageProcessingStage.Handle
                }
            };

            var metadataProvider = new MetadataProvider(new IMetadataSource[]
            {
                new PerformedOperationsMessageFlowsMetadataSource()
            },
            new IMetadataProcessor[]
            {
                new ReferencesEvaluatorProcessor()
            });

            var receiver = new ServiceBusOperationsReceiver(new TrackedUseCaseParser(), messageBytes);

            return container
                        .RegisterType<ITrackedUseCaseParser, TrackedUseCaseParser>(Lifetime.Singleton)

                        .RegisterType<ITracer, NullTracer>()
                        .RegisterInstance<IMetadataProvider>(metadataProvider)
                        .RegisterInstance<IPerformedOperationsFlowProcessorSettings>(settings)
                        .RegisterInstance(receiver)
                        .RegisterType<IMessageReceiverFactory, UnityMessageReceiverFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageFlowProcessorResolveStrategy, Mocks.Processor.PerformedOperationsPrimaryProcessingStrategy>("primary", Lifetime.Singleton)
                        .RegisterType<IMessageFlowReceiverResolveStrategy, Mocks.Receiver.PerformedOperationsPrimaryProcessingStrategy>("primary", Lifetime.Singleton)
                        .RegisterType<IMessageProcessingStagesFactory, UnityMessageProcessingStagesFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageTransformerFactory, UnityMessageTransformerFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageFlowProcessorFactory, UnityMessageFlowProcessorFactory>(Lifetime.Singleton)
                        .RegisterType<IParentContainerUsedRegistrationsContainer, ParentContainerUsedRegistrationsContainer>(Lifetime.Singleton)
                        .RegisterType<IMessageAggregatedProcessingResultsHandlerFactory, UnityMessageAggregatedProcessingResultsHandlerFactory>(Lifetime.PerScope)
                        .RegisterType<IMessageProcessingStrategyFactory, UnityMessageProcessingStrategyFactory>(Lifetime.PerScope);
        }
    }
}
