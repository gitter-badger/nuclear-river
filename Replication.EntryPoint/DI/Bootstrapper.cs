using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.EntryPoint.Factories;
using NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Processor;
using NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Receiver;
using NuClear.AdvancedSearch.Replication.EntryPoint.Factories.Messaging.Transformer;
using NuClear.Assembling.TypeProcessing;
using NuClear.DI.Unity.Config;
using NuClear.DI.Unity.Config.RegistrationResolvers;
using NuClear.Jobs.Schedulers;
using NuClear.Jobs.Unity;
using NuClear.Messaging.API.Processing.Actors.Accumulators;
using NuClear.Messaging.API.Processing.Actors.Handlers;
using NuClear.Messaging.API.Processing.Actors.Transformers;
using NuClear.Messaging.API.Processing.Actors.Validators;
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
using NuClear.Messaging.DI.Factories.Unity.Transformers.Resolvers;
using NuClear.Messaging.DI.Factories.Unity.Validators;
using NuClear.Messaging.Transports.CorporateBus;
using NuClear.Messaging.Transports.CorporateBus.API;
using NuClear.Messaging.Transports.ServiceBus;
using NuClear.Messaging.Transports.ServiceBus.API;
using NuClear.Metamodeling.Domain.Processors.Concrete;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Processors.Concrete;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Metamodeling.Validators;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf.Surrogates;
using NuClear.OperationsProcessing.Transports.ServiceBus.Primary;
using NuClear.Replication.OperationsProcessing.Final;
using NuClear.Replication.OperationsProcessing.Metadata.Flows;
using NuClear.Replication.OperationsProcessing.Metadata.Model;
using NuClear.Replication.OperationsProcessing.Performance;
using NuClear.Replication.OperationsProcessing.Transports.CorporateBus;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.Security;
using NuClear.Security.API;
using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;
using NuClear.Settings.API;
using NuClear.Settings.Unity;
using NuClear.Telemetry;
using NuClear.Tracing.API;
using NuClear.WCF.Client;
using NuClear.WCF.Client.Config;

using Quartz.Spi;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
{
    public static partial class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(ISettingsContainer settingsContainer, ITracer tracer, ITracerContextManager tracerContextManager)
        {
            EntityTypeMap.Initialize();

            IUnityContainer container = new UnityContainer();
            var massProcessors = new IMassProcessor[]
                                 {
                                     new TaskServiceJobsMassProcessor(container),
                                 };

            container.AttachQueryableContainerExtension()
                     .UseParameterResolvers(ParameterResolvers.Defaults)
                     .ConfigureMetadata()
                     .ConfigureSettingsAspects(settingsContainer)
                     .ConfigureTracing(tracer, tracerContextManager)
                     .ConfigureSecurityAspects()
                     .ConfigureQuartz()
                     .ConfigureOperationsProcessing()
                     .ConfigureWcf()
                     .ConfigureLinq2Db();

            ReplicationRoot.Instance.PerformTypesMassProcessing(massProcessors, true, typeof(object));

            return container;
        }

        public static IUnityContainer ConfigureTracing(this IUnityContainer container, ITracer tracer, ITracerContextManager tracerContextManager)
        {
            return container.RegisterInstance(tracer)
                            .RegisterInstance(tracerContextManager);
        }

        public static IUnityContainer ConfigureMetadata(this IUnityContainer container)
        {
            // provider
            container.RegisterType<IMetadataProvider, MetadataProvider>(Lifetime.Singleton);

            // processors
            container.RegisterOne2ManyTypesPerTypeUniqueness<IMetadataProcessor, ReferencesEvaluatorProcessor>(Lifetime.Singleton);
            container.RegisterOne2ManyTypesPerTypeUniqueness<IMetadataProcessor, Feature2PropertiesLinkerProcessor>(Lifetime.Singleton);

            // validators
            container.RegisterType<IMetadataValidatorsSuite, MetadataValidatorsSuite>(Lifetime.Singleton);

            // register matadata sources without massprocessor
            container.RegisterOne2ManyTypesPerTypeUniqueness(typeof(IMetadataSource), typeof(PerformedOperationsMessageFlowsMetadataSource), Lifetime.Singleton);

            return container;
        }

        private static IUnityContainer ConfigureSecurityAspects(this IUnityContainer container)
        {
            return container
                .RegisterType<IUserAuthenticationService, NullUserAuthenticationService>(Lifetime.PerScope)
                .RegisterType<IUserProfileService, NullUserProfileService>(Lifetime.PerScope)
                .RegisterType<IUserContext, UserContext>(Lifetime.PerScope, new InjectionFactory(c => new UserContext(null, null)))
                .RegisterType<IUserLogonAuditor, LoggerContextUserLogonAuditor>(Lifetime.Singleton)
                .RegisterType<IUserIdentityLogonService, UserIdentityLogonService>(Lifetime.PerScope)
                .RegisterType<ISignInService, WindowsIdentitySignInService>(Lifetime.PerScope)
                .RegisterType<IUserImpersonationService, UserImpersonationService>(Lifetime.PerScope);
        }

        private static IUnityContainer ConfigureQuartz(this IUnityContainer container)
        {
            return container
                .RegisterType<IJobFactory, JobFactory>(Lifetime.Singleton, new InjectionConstructor(container.Resolve<UnityJobFactory>(), container.Resolve<ITracer>()))
                .RegisterType<IJobStoreFactory, JobStoreFactory>(Lifetime.Singleton)
                .RegisterType<ISchedulerManager, SchedulerManager>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureWcf(this IUnityContainer container)
        {
            return container
                .RegisterType<IServiceClientSettingsProvider, ServiceClientSettingsProvider>(Lifetime.Singleton)
                .RegisterType<IClientProxyFactory, ClientProxyFactory>(Lifetime.Singleton);
        }

        private static IUnityContainer ConfigureOperationsProcessing(this IUnityContainer container)
        {
            IdentitySurrogate.SetResolver(x => container.Resolve(x));
            container.RegisterType<ITelemetryPublisher, AggregatingTelemetryPublisherDecorator>(Lifetime.Singleton,
                                                                 new InjectionConstructor(
                                                                     new ResolvedArrayParameter<ITelemetryPublisher>(
                                                                         new ResolvedParameter<DebugTelemetryPublisher>(),
                                                                         new ResolvedParameter<LogstashTelemetryPublisher>())));

            // primary
            container
                     .RegisterTypeWithDependencies(typeof(CorporateBusOperationsReceiver), Lifetime.PerScope, null)
                     .RegisterTypeWithDependencies(typeof(ServiceBusOperationsReceiverTelemetryWrapper), Lifetime.PerScope, null)
                     .RegisterOne2ManyTypesPerTypeUniqueness<IRuntimeTypeModelConfigurator, ProtoBufTypeModelForTrackedUseCaseConfigurator>(Lifetime.Singleton)
                     .RegisterOne2ManyTypesPerTypeUniqueness<IRuntimeTypeModelConfigurator, TrackedUseCaseConfigurator>(Lifetime.Singleton)
                     .RegisterTypeWithDependencies(typeof(BinaryEntireBrokeredMessage2TrackedUseCaseTransformer), Lifetime.Singleton, null);

            // final
            container.RegisterTypeWithDependencies(typeof(SqlStoreReceiverTelemetryWrapper), Lifetime.PerScope, null)
                     .RegisterTypeWithDependencies(typeof(AggregateOperationAggregatableMessageHandler), Lifetime.PerResolve, null);


            return container.RegisterInstance<IParentContainerUsedRegistrationsContainer>(new ParentContainerUsedRegistrationsContainer(typeof(IUserContext)), Lifetime.Singleton)
                            .RegisterType(typeof(ServiceBusMessageFlowReceiver), Lifetime.Singleton)
                            .RegisterType(typeof(CorporateBusMessageFlowReceiver), Lifetime.PerResolve)
                            .RegisterType<ICorporateBusMessageFlowReceiverFactory, UnityCorporateBusMessageFlowReceiverFactory>(Lifetime.Singleton)
                            .RegisterType<IServiceBusMessageFlowReceiverFactory, UnityServiceBusMessageFlowReceiverFactory>(Lifetime.Singleton)
                            .RegisterType<IMessageProcessingStagesFactory, UnityMessageProcessingStagesFactory>(Lifetime.Singleton)
                            .RegisterType<IMessageFlowProcessorFactory, UnityMessageFlowProcessorFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageReceiverFactory, UnityMessageReceiverFactory>(Lifetime.PerScope)

                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowProcessorResolveStrategy, PrimaryProcessorResolveStrategy>(Lifetime.Singleton)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowProcessorResolveStrategy, FinalProcessorResolveStrategy>(Lifetime.PerScope)

                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowReceiverResolveStrategy, CorporateBusReceiverResolveStrategy>(Lifetime.PerScope)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowReceiverResolveStrategy, ServiceBusReceiverResolveStrategy>(Lifetime.PerScope)
                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageFlowReceiverResolveStrategy, FinalReceiverResolveStrategy>(Lifetime.PerScope)

                            .RegisterType<IMessageValidatorFactory, UnityMessageValidatorFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageTransformerFactory, UnityMessageTransformerFactory>(Lifetime.PerScope)

                            .RegisterOne2ManyTypesPerTypeUniqueness<IMessageTransformerResolveStrategy, PrimaryMessageTransformerResolveStrategy>(Lifetime.PerScope)
                            .RegisterType<IMessageProcessingHandlerFactory, UnityMessageProcessingHandlerFactory>(Lifetime.PerScope)
                            .RegisterType<IMessageProcessingContextAccumulatorFactory, UnityMessageProcessingContextAccumulatorFactory>(Lifetime.PerScope);
        }
    }
}