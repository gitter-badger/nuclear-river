using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.Replication.Metadata.Aggregates
{
    public interface IValueObjectFeature : IMetadataFeature
    {
         Type ValueObjectType { get; }
    }
}