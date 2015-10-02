using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.AdvancedSearch.Common.Metadata.Features
{
    public interface IValueObjectFeature : IMetadataFeature
    {
         Type ValueObjectType { get; }
    }
}