using NuClear.Replication.Metadata.Facts;

namespace NuClear.Replication.Core.API.Facts
{
    public interface IFactDependencyProcessorFactory
    {
        IFactDependencyProcessor Create(IFactDependencyFeature metadata);
    }
}