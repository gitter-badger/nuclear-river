using System;
using System.Configuration;
using System.Data.Common;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Web.OData.DataAccess
{
    public sealed class ODataConnectionFactory
    {
        private const string CommonConnectionStringName = "CustomerIntelligence";

        private readonly IMetadataProvider _metadataProvider;

        public ODataConnectionFactory(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public DbConnection CreateConnection(Uri contextId)
        {
            BoundedContextElement contextElement;
            if (!_metadataProvider.TryGetMetadata(contextId, out contextElement))
            {
                return null;
            }

            // далее можно кастомизовать DbConnection используя contextId, но пока это не нужно
            var commonConnectionString = ConfigurationManager.ConnectionStrings[CommonConnectionStringName];
            return CreateConnection(commonConnectionString);
        }

        private static DbConnection CreateConnection(ConnectionStringSettings connectionStringSettings)
        {
            var dbProviderFactory = DbProviderFactories.GetFactory(connectionStringSettings.ProviderName);
            var dbConection = dbProviderFactory.CreateConnection();
            if (dbConection == null)
            {
                throw new ArgumentException();
            }

            dbConection.ConnectionString = connectionStringSettings.ConnectionString;
            return dbConection;
        }
    }
}