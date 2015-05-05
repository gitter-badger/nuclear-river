using LinqToDB;
using LinqToDB.Data;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.DI.Unity.Config;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
{
    public static partial class Bootstrapper
    {
        private static IUnityContainer ConfigureLinq2Db(this IUnityContainer container)
        {
            return container
                .RegisterType<IDataContext, DataConnection>("Erm", Lifetime.PerScope, new InjectionFactory(c => new DataConnection("Erm").AddMappingSchema(Schema.Erm)))
                .RegisterType<IDataContext, DataConnection>("Facts", Lifetime.PerScope, new InjectionFactory(c => new DataConnection("CustomerIntelligence").AddMappingSchema(Schema.Facts)))
                .RegisterType<IDataContext, DataConnection>("CustomerIntelligence", Lifetime.PerScope, new InjectionFactory(c => new DataConnection("CustomerIntelligence").AddMappingSchema(Schema.CustomerIntelligence)))

                .RegisterType<IDataMapper, DataMapper>("Facts", Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>("Facts")))

                .RegisterType<IErmContext, ErmContext>(Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>("Erm")))
                .RegisterType<IFactsContext, FactsContext>("Facts", Lifetime.PerScope, new InjectionConstructor(new ResolvedParameter<IDataContext>("Facts")))
                .RegisterType<IFactsContext, FactsTransformationContext>("FactsTransform", Lifetime.PerScope)

                .RegisterType<FactsTransformation>(Lifetime.PerScope,
                                                   new InjectionConstructor(
                                                       new ResolvedParameter<IFactsContext>("FactsTransform"),
                                                       new ResolvedParameter<IFactsContext>("Facts"),
                                                       new ResolvedParameter<IDataMapper>("Facts")));
        }

    }
}