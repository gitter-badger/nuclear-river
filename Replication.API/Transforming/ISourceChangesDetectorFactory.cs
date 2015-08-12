using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface ISourceChangesDetectorFactory
    {
        ISourceChangesDetector Create(IFactInfo factInfo, IQuery source, IQuery target);
    }
}