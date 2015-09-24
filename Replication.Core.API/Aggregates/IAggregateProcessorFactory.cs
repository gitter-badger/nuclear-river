using System;

using NuClear.Metamodeling.Elements;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IAggregateProcessorFactory
    {
        IAggregateProcessor Create(Type aggregateType, IMetadataElement aggregateMetadata);
    }
}