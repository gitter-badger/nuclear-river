using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;

using Microsoft.Practices.Unity;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.EntryPoint.Settings;

namespace NuClear.AdvancedSearch.Replication.EntryPoint.DI
{
    public static partial class Bootstrapper
    {
        public static IUnityContainer ConfigureTransformation(this IUnityContainer container)
        {
            return container
                .RegisterType<IDataContext>(ConnectionStringName.Erm.ToString(),
                                            new InjectionFactory(x => CreateInstance(container, ConnectionStringName.Erm, Schema.Erm)))
                .RegisterType<IDataContext>(ConnectionStringName.Facts.ToString(),
                                            new InjectionFactory(x => CreateInstance(container, ConnectionStringName.Facts, Schema.Facts)))
                .RegisterType<IDataContext>(ConnectionStringName.CustomerIntelligence.ToString(),
                                            new InjectionFactory(x => CreateInstance(container, ConnectionStringName.CustomerIntelligence, Schema.CustomerIntelligence)))

                .RegisterType<IErmContext, ErmContext>()
                .RegisterType<FactsTransformation>(new InjectionFactory(CreateFactsTransformation));
        }

        private static FactsTransformation CreateFactsTransformation(IUnityContainer container)
        {
            var ermDataContext = container.Resolve<IDataContext>(ConnectionStringName.Erm.ToString());
            var factDataContext = container.Resolve<IDataContext>(ConnectionStringName.Facts.ToString());

            var source = container.Resolve<FactsTransformationContext>(new DependencyOverride(typeof(IDataContext), ermDataContext));
            var target = container.Resolve<FactsContext>(new DependencyOverride(typeof(IDataContext), factDataContext));
            var mapper = container.Resolve<DataMapper>(new DependencyOverride(typeof(IDataContext), factDataContext));

            return new FactsTransformation(source, target, mapper);
        }

        private static IDataContext CreateInstance(IUnityContainer container, ConnectionStringName connectionName, MappingSchema schema)
        {
            var settings = container.Resolve<IConnectionStringSettings>();
            var connectionSettings = settings.GetConnectionStringSettings(connectionName);

            var provider = DataConnection.GetDataProvider(connectionSettings.Name);
            var connection = provider.CreateConnection(connectionSettings.ConnectionString);
            connection.Open();

            return new DataConnection(provider, connection).AddMappingSchema(schema);
        }
    }
}