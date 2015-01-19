using System;

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

        public EdmModelFactory(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public IEdmModel Create(string name)
        {
            BoundedContextElement contextElement;

            var id = IdBuilder.For<AdvancedSearchIdentity>(name);
            if (!_metadataProvider.TryGetMetadata(id, out contextElement))
            {
                throw new InvalidOperationException("The specified name does not belong to an existing bounded context.");
            }

            return EdmModelBuilder.Build(contextElement);
        }
    }
}
