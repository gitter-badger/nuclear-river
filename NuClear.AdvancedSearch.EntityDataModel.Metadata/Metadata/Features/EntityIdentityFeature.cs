using System;

using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Features
{
    public sealed class ElementMappingFeature : IUniqueMetadataFeature
    {
        private readonly IMetadataElementIdentity _mappedElementIdentity;

        public ElementMappingFeature(IMetadataElementIdentity mappedElementIdentity)
        {
            if (mappedElementIdentity == null)
            {
                throw new ArgumentNullException("mappedElementIdentity");
            }
            _mappedElementIdentity = mappedElementIdentity;
        }

        public IMetadataElementIdentity MappedElementIdentity
        {
            get
            {
                return _mappedElementIdentity;
            }
        }
    }
}