using System;

using NuClear.Metamodeling.Elements;

namespace NuClear.AdvancedSearch.Common.Metadata.Elements
{
    public interface IValueObjectMetadataElement : IMetadataElement
    {
         Type ValueObjectType { get; }
    }
}