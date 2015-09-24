using NuClear.Replication.Metadata.Aggregates;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IValueObjectProcessorFactory
    {
        IValueObjectProcessor Create(IValueObjectFeature metadata);
    }
}