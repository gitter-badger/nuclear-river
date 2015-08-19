using System.Linq;

using LinqToDB;
using LinqToDB.Common;
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

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
{
    using TransportSchema = Schema;

    public static partial class Bootstrapper
    {
        private static IUnityContainer ConfigureLinq2Db(this IUnityContainer container)
        {
            // разрешаем update на таблицу состоящую только из Primary Keys
            Configuration.Linq.IgnoreEmptyUpdate = true;

            var sqlSettings = container.Resolve<ISqlSettingsAspect>();

            return container
                .RegisterDataContext(Scope.Facts, ConnectionStringName.CustomerIntelligence, CustomerIntelligence.Data.Schema.Facts, sqlSettings.SqlCommandTimeout)
                .RegisterDataContext(Scope.Transport, ConnectionStringName.CustomerIntelligence, TransportSchema.Transport, sqlSettings.SqlCommandTimeout)

                .RegisterType<IDataMapper, DataMapper>(Scope.Facts, Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Facts)))
                .RegisterType<IDataMapper, DataMapper>(Scope.CustomerIntelligence, Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.CustomerIntelligence)))

                .RegisterType<StatisticsContext>(Lifetime.PerScope, 
                    new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.CustomerIntelligence)))
                .RegisterType<StatisticsTransformationContext>(Lifetime.PerScope,
                    new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Facts)))


                .RegisterType<CustomerIntelligenceTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<IQuery>(),
                                                       new ResolvedParameter<IDataMapper>(Scope.CustomerIntelligence),
                                                       ResolvedTransactionManager(container, Scope.Facts, Scope.CustomerIntelligence)))

                .RegisterType<BitFactsTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<IQuery>(Scope.Facts),
                                                       new ResolvedParameter<IDataMapper>(Scope.Facts),
                                                       ResolvedTransactionManager(container, Scope.Facts)))
                                                      
                .RegisterType<StatisticsFinalTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<StatisticsTransformationContext>(),
                                                       new ResolvedParameter<StatisticsContext>(),
                                                       new ResolvedParameter<IDataMapper>(Scope.CustomerIntelligence)))

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