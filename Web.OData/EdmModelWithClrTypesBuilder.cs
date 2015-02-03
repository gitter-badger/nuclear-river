using System;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Building;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.AdvancedSearch.QueryExecution;
using NuClear.AdvancedSearch.Web.OData.DataAccess;

namespace NuClear.AdvancedSearch.Web.OData
{
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