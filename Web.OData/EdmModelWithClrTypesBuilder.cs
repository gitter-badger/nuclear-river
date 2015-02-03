using System;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Building;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.AdvancedSearch.QueryExecution;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Web.OData
{
    public sealed class ODataConnectionFactory
    {
        private const string CommonConnectionStringName = "ODATA";

        private readonly IMetadataProvider _metadataProvider;

        public ODataConnectionFactory(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public DbConnection CreateConnection(Uri identity)
        {
            BoundedContextElement contextElement;
            if (!_metadataProvider.TryGetMetadata(identity, out contextElement))
            {
                return null;
            }

            // абстракция от sql provider должна быть, а её пока что нету
            var commonConnectionString = ConfigurationManager.ConnectionStrings[CommonConnectionStringName].ConnectionString;
            return new SqlConnection(commonConnectionString);
        }
    }

    public sealed class ODataDbContext : DbContext
    {
        public ODataDbContext(IEdmModel model, ODataConnectionFactory connectionFactory)
            : base(connectionFactory.CreateConnection(model.GetMetadataIdentity()), model.GetDbCompiledModel(), true)
        {
            Database.CommandTimeout = 60;
        }
    }

    public sealed class EdmModelWithClrTypesBuilder
    {
        private readonly EdmModelBuilder _edmModelBuilder;
        private readonly EdmxModelBuilder _edmxModelBuilder;
        private readonly ODataConnectionFactory _connectionFactory;

        public EdmModelWithClrTypesBuilder(EdmModelBuilder edmModelBuilder, EdmxModelBuilder edmxModelBuilder, ODataConnectionFactory connectionFactory)
        {
            _edmModelBuilder = edmModelBuilder;
            _edmxModelBuilder = edmxModelBuilder;
            _connectionFactory = connectionFactory;
        }

        public IEdmModel Build(Uri uri)
        {
            var edmModel = _edmModelBuilder.Build(uri);
            edmModel.AddMetadataIdentityAnnotation(uri);

            var connection = _connectionFactory.CreateConnection(uri);
            var edmxModel = _edmxModelBuilder.Build(connection, uri);
            var clrTypes = edmxModel.GetClrTypes();
            edmModel.AddClrAnnotations(clrTypes);
            edmModel.AddDbCompiledModelAnnotation(edmxModel.Compile());

            return edmModel;
        }
   }
}