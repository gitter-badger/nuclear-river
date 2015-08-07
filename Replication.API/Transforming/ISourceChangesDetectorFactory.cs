using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    public interface ISourceChangesDetectorFactory
    {
        ISourceChangesDetector Create(ErmFactInfo factInfo, IQuery sourceQuery, IQuery destQuery);
    }
}