using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Settings;
using NuClear.DI.Unity.Config;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.OperationsProcessing.API.Final;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;

using Schema = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
{
    using TransportSchema = NuClear.Replication.OperationsProcessing.Transports.SQLStore.Schema;

    public static partial class Bootstrapper
    {
        private static IUnityContainer ConfigureLinq2Db(this IUnityContainer container)
        {
            return container
                .RegisterDataContext(Scope.Erm, ConnectionStringName.Erm, Schema.Erm)
                .RegisterDataContext(Scope.Facts, ConnectionStringName.CustomerIntelligence, Schema.Facts)
                .RegisterDataContext(Scope.CustomerIntelligence, ConnectionStringName.CustomerIntelligence, Schema.CustomerIntelligence)
                .RegisterDataContext(Scope.Transport, ConnectionStringName.CustomerIntelligence, TransportSchema.Transport)

                .RegisterType<IDataMapper, DataMapper>(Scope.Facts, Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Facts)))
                .RegisterType<IDataMapper, DataMapper>(Scope.CustomerIntelligence, Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.CustomerIntelligence)))

                .RegisterType<IErmContext, ErmContext>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Erm)))

                .RegisterType<ErmFactsContext>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Facts)))
                .RegisterType<ErmFactsTransformationContext>(Lifetime.PerScope)

                .RegisterType<BitFactsContext>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Facts)))
                // No BitTransformationContext registration, it depends on dto

                .RegisterType<CustomerIntelligenceContext>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.CustomerIntelligence)))
                .RegisterType<CustomerIntelligenceTransformationContext>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<ErmFactsContext>(), new ResolvedParameter<BitFactsContext>()))


                .RegisterType<ErmFactsTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<ErmFactsTransformationContext>(),
                                                       new ResolvedParameter<ErmFactsContext>(),
                                                       new ResolvedParameter<IDataMapper>(Scope.Facts)))

                .RegisterType<CustomerIntelligenceTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<CustomerIntelligenceTransformationContext>(),
                                                       new ResolvedParameter<CustomerIntelligenceContext>(),
                                                       new ResolvedParameter<IDataMapper>(Scope.CustomerIntelligence)))

                .RegisterType<BitFactsTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<BitFactsContext>(),
                                                       new ResolvedParameter<IDataMapper>(Scope.Facts)))

                .RegisterType<SqlStoreSender>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Transport)))
                .RegisterType<SqlStoreReceiver>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<MessageFlowMetadata>(), new ResolvedParameter<IFinalProcessingQueueReceiverSettings>(), new ResolvedParameter<IDataContext>(Scope.Transport)));
        }

        private static IUnityContainer RegisterDataContext(this IUnityContainer container, string scope, ConnectionStringName connectionStringName, MappingSchema schema)
        {
            return container.RegisterType<IDataContext, DataConnection>(scope, 
                Lifetime.PerScope,
                new InjectionFactory(c => new DataConnection(connectionStringName.ToString()).AddMappingSchema(schema)));
        }

        private static class Scope
        {
            public const string Erm = "Erm";
            public const string Facts = "Facts";
            public const string CustomerIntelligence = "CustomerIntelligence";
            public const string Transport = "Transport";
        }
    }
}