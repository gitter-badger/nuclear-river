using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.EntityFramework.Building;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.AdvancedSearch.QueryExecution;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.Web.OData.Model
{
    public sealed class TypedEdmModelBuilder
    {
        private readonly EdmModelBuilder _edmModelBuilder;
        private readonly EdmxModelBuilder _edmxModelBuilder;

        public TypedEdmModelBuilder(EdmModelBuilder edmModelBuilder, EdmxModelBuilder edmxModelBuilder)
        {
            _edmModelBuilder = edmModelBuilder;
            _edmxModelBuilder = edmxModelBuilder;
        }

        public IEdmModel Build(string name)
        {
            var uri = IdBuilder.For<AdvancedSearchIdentity>(name);

            var edmxModel = _edmxModelBuilder.Build(uri);
            var clrTypes = edmxModel.GetClrTypes();

            var edmModel = _edmModelBuilder.Build(uri);
            edmModel.AddClrAnnotations(clrTypes);

            return edmModel;
        }
    }
}