using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.DataTest.Metamodel
{
    public class EntityTypeFeature : IMetadataFeature
    {
        public EntityTypeFeature(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}