using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.OData;
using NuClear.AdvancedSearch.QueryExecution;
using NuClear.EntityDataModel.EntityFramework.Building;

namespace NuClear.AdvancedSearch.Web.OData.Model
{
    public sealed class TypedEdmModelFactory
    {
        private readonly EdmModelFactory _edmModelFactory;
        private readonly EdmxModelBuilder _edmxModelBuilder;
        private readonly IEdmModelTypesMapper _edmModelTypesMapper;

        public TypedEdmModelFactory(EdmModelFactory edmModelFactory, EdmxModelBuilder edmxModelBuilder, IEdmModelTypesMapper edmModelTypesMapper)
        {
            _edmModelFactory = edmModelFactory;
            _edmxModelBuilder = edmxModelBuilder;
            _edmModelTypesMapper = edmModelTypesMapper;
        }

        public IEdmModel Create(string name)
        {
            var edmModel = _edmModelFactory.Create(name);

            _edmxModelBuilder.Build()

            return _edmModelTypesMapper.MapTypes(edmModel);
        }
    }
}