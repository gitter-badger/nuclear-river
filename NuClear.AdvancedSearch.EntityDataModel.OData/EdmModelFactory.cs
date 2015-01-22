using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.EntityDataModel.OData
{
    public sealed class EdmModelFactory
    {
        private readonly EdmModelBuilder _modelBuilder;

        public EdmModelFactory(IMetadataProvider metadataProvider)
        {
            _modelBuilder = new EdmModelBuilder(metadataProvider);
        }

        public IEdmModel Create(string name)
        {
            return _modelBuilder.Build(IdBuilder.For<AdvancedSearchIdentity>(name));
        }
    }
}
