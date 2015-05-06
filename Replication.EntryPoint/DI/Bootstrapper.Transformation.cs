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
                .RegisterType<IDataContext, DataConnection>("Erm", Lifetime.PerScope, new InjectionFactory(c => new DataConnection("Erm").AddMappingSchema(Schema.Erm)))
                .RegisterType<IDataContext, DataConnection>("Facts", Lifetime.PerScope, new InjectionFactory(c => new DataConnection("CustomerIntelligence").AddMappingSchema(Schema.Facts)))
                .RegisterType<IDataContext, DataConnection>("CustomerIntelligence", Lifetime.PerScope, new InjectionFactory(c => new DataConnection("CustomerIntelligence").AddMappingSchema(Schema.CustomerIntelligence)))
                .RegisterType<IDataContext, DataConnection>("Transport", Lifetime.PerScope, new InjectionFactory(c => new DataConnection("CustomerIntelligence").AddMappingSchema(TransportSchema.Transport)))

                .RegisterType<IDataMapper, DataMapper>("Facts", Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>("Facts")))
                .RegisterType<IDataMapper, DataMapper>("CustomerIntelligence", Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>("CustomerIntelligence")))

                .RegisterType<IErmContext, ErmContext>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>("Erm")))

                .RegisterType<IFactsContext, FactsContext>("Facts", Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>("Facts")))
                .RegisterType<IFactsContext, FactsTransformationContext>("FactsTransform", Lifetime.PerScope)

                .RegisterType<ICustomerIntelligenceContext, CustomerIntelligenceContext>("CustomerIntelligence", Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>("Facts")))
                .RegisterType<ICustomerIntelligenceContext, CustomerIntelligenceTransformationContext>("CustomerIntelligenceTransform", Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IFactsContext>("Facts")))


                .RegisterType<FactsTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<IFactsContext>("FactsTransform"),
                                                       new ResolvedParameter<IFactsContext>("Facts"),
                                                       new ResolvedParameter<IDataMapper>("Facts")))

                .RegisterType<CustomerIntelligenceTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<ICustomerIntelligenceContext>("CustomerIntelligenceTransform"),
                                                       new ResolvedParameter<ICustomerIntelligenceContext>("CustomerIntelligence"),
                                                       new ResolvedParameter<IDataMapper>("CustomerIntelligence")))

                .RegisterType<SqlStoreSender>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>("Transport")))
                .RegisterType<SqlStoreReceiver>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>("Transport")));
        }
    }
}