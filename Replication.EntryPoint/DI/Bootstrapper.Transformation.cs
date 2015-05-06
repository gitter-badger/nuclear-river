using LinqToDB;
using LinqToDB.Data;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.DI.Unity.Config;
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
                .RegisterType<IDataContext, DataConnection>(Scope.Erm, Lifetime.PerScope, new InjectionFactory(c => new DataConnection(Connection.Erm).AddMappingSchema(Schema.Erm)))
                .RegisterType<IDataContext, DataConnection>(Scope.Facts, Lifetime.PerScope, new InjectionFactory(c => new DataConnection(Connection.CustomerIntelligence).AddMappingSchema(Schema.Facts)))
                .RegisterType<IDataContext, DataConnection>(Scope.CustomerIntelligence, Lifetime.PerScope, new InjectionFactory(c => new DataConnection(Connection.CustomerIntelligence).AddMappingSchema(Schema.CustomerIntelligence)))
                .RegisterType<IDataContext, DataConnection>(Scope.Transport, Lifetime.PerScope, new InjectionFactory(c => new DataConnection(Connection.CustomerIntelligence).AddMappingSchema(TransportSchema.Transport)))

                .RegisterType<IDataMapper, DataMapper>(Scope.Facts, Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Facts)))
                .RegisterType<IDataMapper, DataMapper>(Scope.CustomerIntelligence, Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.CustomerIntelligence)))

                .RegisterType<IErmContext, ErmContext>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Erm)))

                .RegisterType<IFactsContext, FactsContext>(Scope.Facts, Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Facts)))
                .RegisterType<IFactsContext, FactsTransformationContext>("FactsTransform", Lifetime.PerScope)

                .RegisterType<ICustomerIntelligenceContext, CustomerIntelligenceContext>(Scope.CustomerIntelligence, Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.CustomerIntelligence)))
                .RegisterType<ICustomerIntelligenceContext, CustomerIntelligenceTransformationContext>("CustomerIntelligenceTransform", Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IFactsContext>(Scope.Facts)))


                .RegisterType<FactsTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<IFactsContext>("FactsTransform"),
                                                       new ResolvedParameter<IFactsContext>(Scope.Facts),
                                                       new ResolvedParameter<IDataMapper>(Scope.Facts)))

                .RegisterType<CustomerIntelligenceTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<ICustomerIntelligenceContext>("CustomerIntelligenceTransform"),
                                                       new ResolvedParameter<ICustomerIntelligenceContext>(Scope.CustomerIntelligence),
                                                       new ResolvedParameter<IDataMapper>(Scope.CustomerIntelligence)))

                .RegisterType<SqlStoreSender>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Transport)))
                .RegisterType<SqlStoreReceiver>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>(Scope.Transport)));
        }

        private static class Scope
        {
            public const string Erm = "Erm";
            public const string Facts = "Facts";
            public const string CustomerIntelligence = "CustomerIntelligence";
            public const string Transport = "Transport";
        }

        private static class Connection
        {
            public const string Erm = "Erm";
            public const string CustomerIntelligence = "CustomerIntelligence";
        }
    }
}