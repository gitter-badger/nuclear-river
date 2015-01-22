using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Features
{
    public sealed class EntityIdentityFeature : IUniqueMetadataFeature
    {
        private readonly EntityPropertyElement[] _identifyingProperties;

        public EntityIdentityFeature(params EntityPropertyElement[] identifyingProperties)
        {
            if (identifyingProperties == null)
            {
                throw new ArgumentNullException("identifyingProperties");
            }
            if (identifyingProperties.Length == 0)
            {
                throw new ArgumentException("The properties should be provided.", "identifyingProperties");
            }

            _identifyingProperties = identifyingProperties;
        }

        public IReadOnlyCollection<EntityPropertyElement> IdentifyingProperties
        {
            get
            {
                return _identifyingProperties;
            }
        }
    }
}