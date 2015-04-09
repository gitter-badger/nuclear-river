using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Messaging.Metadata.Flows;
using NuClear.AdvancedSearch.Messaging.Tests.Mocks;
using NuClear.DI.Unity.Config;
using NuClear.Messaging.API.Processing.Actors.Strategies;
using NuClear.Messaging.API.Processing.Actors.Transformers;
using NuClear.Messaging.API.Processing.Processors;
using NuClear.Messaging.API.Processing.Stages;
using NuClear.Messaging.API.Receivers;
using NuClear.Messaging.DI.Factories.Unity.Accumulators;
using NuClear.Messaging.DI.Factories.Unity.Processors;
using NuClear.Messaging.DI.Factories.Unity.Processors.Resolvers;
using NuClear.Messaging.DI.Factories.Unity.Receivers;
using NuClear.Messaging.DI.Factories.Unity.Stages;
using NuClear.Messaging.DI.Factories.Unity.Transformers;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.OperationsProcessing.API;
using NuClear.OperationsProcessing.API.Primary;
using NuClear.Tracing.API;

namespace NuClear.AdvancedSearch.Messaging.Tests.DI
{
    public static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(this IUnityContainer container, byte[] messageBytes)
        {
            var settings = new PerformedOperationsPrimaryFlowProcessorSettings
            {
                AppropriatedStages = new MessageProcessingStage[0], //new[]
                //{
                //MessageProcessingStage.Transforming, 
                //MessageProcessingStage.Processing, 
                //MessageProcessingStage.Handle
                //}
            };

            var metadataProvider = new MetadataProvider(new IMetadataSource[]
                                                        {
                                                            new PerformedOperationsMessageFlowsMetadataSource()
                                                        }, new IMetadataProcessor[0]);

            var mock1 = new MockServiceBusMessageFlowReceiverFactory(new[] { messageBytes });

            return container
                        .RegisterType<ITracer, NullTracer>()
                        .RegisterInstance<IMetadataProvider>(metadataProvider)
                        .RegisterInstance<IPerformedOperationsFlowProcessorSettings>(settings)
                        .RegisterType<IMessageReceiverFactory, UnityMessageReceiverFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageFlowProcessorResolveStrategy, PerformedOperationsPrimaryProcessingStrategy>("primary", Lifetime.Singleton)
                        .RegisterType<IMessageProcessingStagesFactory, UnityMessageProcessingStagesFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageTransformerFactory, UnityMessageTransformerFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageFlowProcessorFactory, UnityMessageFlowProcessorFactory>(Lifetime.Singleton)
                        .RegisterType<IMessageProcessingStrategyFactory, UnityMessageProcessingStrategyFactory>(Lifetime.PerScope);
        }
    }
}
