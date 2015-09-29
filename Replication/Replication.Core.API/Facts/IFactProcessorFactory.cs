using System;

using NuClear.Metamodeling.Elements;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IFactProcessorFactory
    {
        IFactProcessor Create(IMetadataElement factMetadata);
    }
}