using System;

using Microsoft.OData.Edm;

using NuClear.Querying.EntityFramework.Building;
using NuClear.Querying.OData.Building;
using NuClear.Querying.QueryExecution;
using NuClear.Querying.Web.OData.DataAccess;

namespace NuClear.Querying.Web.OData
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
            edmModel.AnnotateByClrTypes(clrTypes);
            edmModel.AddDbCompiledModelAnnotation(edmxModel.Compile());

            return edmModel;
        }
   }
}