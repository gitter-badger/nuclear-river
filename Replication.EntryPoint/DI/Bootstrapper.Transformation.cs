using System.Linq;

using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Settings;
using NuClear.DI.Unity.Config;
using NuClear.Messaging.API.Flows.Metadata;
using NuClear.OperationsProcessing.API.Final;
using NuClear.Replication.OperationsProcessing.Transports.SQLStore;
using NuClear.Storage.Readings;

using Schema = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Schema;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
{
    using TransportSchema = NuClear.Replication.OperationsProcessing.Transports.SQLStore.Schema;

    public static partial class Bootstrapper
    {
        private static IUnityContainer ConfigureLinq2Db(this IUnityContainer container)
        {
            var sqlSettings = container.Resolve<ISqlSettingsAspect>();

            return container
                .RegisterDataContext(Scope.Erm, ConnectionStringName.Erm, Schema.Erm, sqlSettings.SqlCommandTimeout)
                .RegisterDataContext(Scope.Facts, ConnectionStringName.CustomerIntelligence, Schema.Facts, sqlSettings.SqlCommandTimeout)
                .RegisterDataContext(Scope.CustomerIntelligence, ConnectionStringName.CustomerIntelligence, Schema.CustomerIntelligence, sqlSettings.SqlCommandTimeout)
                .RegisterDataContext(Scope.Transport, ConnectionStringName.CustomerIntelligence, TransportSchema.Transport, sqlSettings.SqlCommandTimeout)

                .RegisterType<IDataMapper, DataMapper>(Scope.Facts, Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Facts)))
                .RegisterType<IDataMapper, DataMapper>(Scope.CustomerIntelligence, Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.CustomerIntelligence)))

                .RegisterType<BitFactsContext>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Facts)))
                // No BitTransformationContext registration, it depends on dto

                .RegisterType<CustomerIntelligenceContext>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.CustomerIntelligence)))
                .RegisterType<CustomerIntelligenceTransformationContext>(Lifetime.PerScope, 
                                                   new InjectionConstructor(new ResolvedParameter<IQuery>(Scope.Facts)))


                .RegisterType<ErmFactsTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<IQuery>(Scope.Erm),
                                                       new ResolvedParameter<IQuery>(Scope.Facts),
                                                       new ResolvedParameter<IDataMapper>(Scope.Facts),
                                                       ResolvedTransactionManager(container, Scope.Erm, Scope.Facts)))

                .RegisterType<CustomerIntelligenceTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<CustomerIntelligenceTransformationContext>(),
                                                       new ResolvedParameter<CustomerIntelligenceContext>(),
                                                       new ResolvedParameter<IDataMapper>(Scope.CustomerIntelligence),
                                                       ResolvedTransactionManager(container, Scope.Facts, Scope.CustomerIntelligence)))

                .RegisterType<BitFactsTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<BitFactsContext>(),
                                                       new ResolvedParameter<IDataMapper>(Scope.Facts),
                                                       ResolvedTransactionManager(container, Scope.Facts)))

                .RegisterType<SqlStoreSender>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Transport)))
                .RegisterType<SqlStoreReceiver>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<MessageFlowMetadata>(), new ResolvedParameter<IFinalProcessingQueueReceiverSettings>(), new ResolvedParameter<IDataContext>(Scope.Transport)));
        }

        private static ResolvedParameter ResolvedTransactionManager(IUnityContainer container, params string[] contexts)
        {
            var specificScope = string.Join("+", contexts);
            if (!container.IsRegistered<ITransactionManager>(specificScope))
            {
                var resolvedContexts = contexts.Select(context => new ResolvedParameter<DataConnection>(context)).ToArray();
                var managerConstructor = new InjectionConstructor(new ResolvedArrayParameter<DataConnection>(resolvedContexts));
                container.RegisterType<ITransactionManager, Linq2DbDataConnectionTransactionManager>(specificScope, managerConstructor);
            }

            return new ResolvedParameter<ITransactionManager>(specificScope);
        }

        private static IUnityContainer RegisterDataContext(this IUnityContainer container, string scope, ConnectionStringName connectionStringName, MappingSchema schema, int timeout)
        {
            return container.RegisterType<IDataContext, DataConnection>(scope, 
                Lifetime.PerScope,
                new InjectionFactory(c =>
                                     {
                                         var connection = new DataConnection(connectionStringName.ToString());
                                         connection.AddMappingSchema(schema);
                                         connection.CommandTimeout = timeout;
                                         return connection;
                                     }));
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