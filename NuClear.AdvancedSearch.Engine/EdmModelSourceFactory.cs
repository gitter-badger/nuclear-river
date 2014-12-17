using System;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;

namespace NuClear.AdvancedSearch.Engine
{
    public sealed class EdmModelSourceFactory
    {
        private readonly IMetadataProvider _metadataProvider;

        public EdmModelSourceFactory(IMetadataProvider metadataProvider)
        {
            _metadataProvider = metadataProvider;
        }

        public IEdmModelSource Create(string name)
        {
            BoundedContextElement contextElement;

            var id = IdBuilder.For<AdvancedSearchIdentity>(name);
            if (!_metadataProvider.TryGetMetadata(id, out contextElement))
            {
                throw new InvalidOperationException("The specified name does not belong to an existing bounded context.");
            }

            return new AdvancedSearchMetadataSourceAdapter(contextElement);
        }
    }
}
