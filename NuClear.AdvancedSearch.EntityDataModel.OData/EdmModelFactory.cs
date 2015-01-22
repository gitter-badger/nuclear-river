using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.EntityDataModel.OData
{
    public sealed class EdmModelFactory
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly EdmModelBuilder _modelBuilder;

        public EdmModelFactory(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
            _modelBuilder = new EdmModelBuilder();
        }

        public IEdmModel Create(string name)
        {
            return _modelBuilder.Build(_metadataProvider, IdBuilder.For<AdvancedSearchIdentity>(name));
        }
    }
}
