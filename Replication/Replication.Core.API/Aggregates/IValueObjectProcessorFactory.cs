using NuClear.AdvancedSearch.Common.Metadata.Features;

namespace NuClear.Replication.Core.API.Aggregates
{
    public interface IValueObjectProcessorFactory
    {
        IValueObjectProcessor Create(IValueObjectFeature metadata);
    }
}