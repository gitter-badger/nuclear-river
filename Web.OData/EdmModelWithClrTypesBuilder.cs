using System;
using System.Data.Entity;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Building;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.AdvancedSearch.QueryExecution;

namespace NuClear.AdvancedSearch.Web.OData
{
    public sealed class EdmModelWithClrTypesBuilder
    {
        private readonly EdmModelBuilder _edmModelBuilder;
        private readonly EdmxModelBuilder _edmxModelBuilder;

        public EdmModelWithClrTypesBuilder(EdmModelBuilder edmModelBuilder, EdmxModelBuilder edmxModelBuilder)
        {
            _edmModelBuilder = edmModelBuilder;
            _edmxModelBuilder = edmxModelBuilder;
        }

        public IEdmModel Build(Uri uri)
        {
            var dbContext = new DbContext("ODATA");
            var connection = dbContext.Database.Connection;

            var edmxModel = _edmxModelBuilder.Build(connection, uri);
            var clrTypes = edmxModel.GetClrTypes();

            var edmModel = _edmModelBuilder.Build(uri);
            edmModel.AddClrAnnotations(clrTypes);
            edmModel.AddDbCompiledModelAnnotation(edmxModel.Compile());

            return edmModel;
        }
   }
}