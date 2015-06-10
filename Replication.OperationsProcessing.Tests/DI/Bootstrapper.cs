using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests.Mocks;
using NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests.Mocks.Receiver;
using NuClear.DI.Unity.Config;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Messaging.API.Processing.Actors.Handlers;
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
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Tracing.API;

namespace NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(this IUnityContainer container, MockMessageReceiver receiver)
        {
            var settings = new PerformedOperationsPrimaryFlowProcessorSettings
            {
                AppropriatedStages = new[]
                {
                    MessageProcessingStage.Transformation,
                    MessageProcessingStage.Accumulation,
                    MessageProcessingStage.Handling
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

            return container
                        .RegisterType<ITrackedUseCaseParser, TrackedUseCaseParser>(Lifetime.Singleton)

                        .RegisterType<ITracer, NullTracer>()
                        .RegisterInstance<IMetadataProvider>(metadataProvider)
                        .RegisterInstance<IPerformedOperationsFlowProcessorSettings>(settings)
                        .RegisterInstance(receiver)
                        .RegisterType<IMessageReceiverFactory, UnityMessageReceiverFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageFlowProcessorResolveStrategy, Mocks.Processor.PrimaryProcessorResolveStrategy>("primary", Lifetime.Singleton)
                        .RegisterType<IMessageFlowReceiverResolveStrategy, MockReceiverResolveStrategy>("primary", Lifetime.Singleton)
                        .RegisterType<IMessageProcessingStagesFactory, UnityMessageProcessingStagesFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageTransformerFactory, UnityMessageTransformerFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageFlowProcessorFactory, UnityMessageFlowProcessorFactory>(Lifetime.Singleton)
                        .RegisterType<IParentContainerUsedRegistrationsContainer, ParentContainerUsedRegistrationsContainer>(Lifetime.Singleton)
                        .RegisterType<IMessageProcessingHandlerFactory, UnityMessageProcessingHandlerFactory>(Lifetime.PerScope)
                        .RegisterType<IMessageProcessingContextAccumulatorFactory, UnityMessageProcessingContextAccumulatorFactory>(Lifetime.PerScope);
        }
    }
}
