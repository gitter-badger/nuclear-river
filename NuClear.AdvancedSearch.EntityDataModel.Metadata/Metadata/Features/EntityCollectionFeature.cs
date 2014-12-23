using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Features
{
    public sealed class EntityCollectionFeature : IUniqueMetadataFeature
    {
        public EntityCollectionFeature(string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException("The collection name should be specified.", "collectionName");
            }
            CollectionName = collectionName;
        }

        public string CollectionName { get; private set; }
    }
}