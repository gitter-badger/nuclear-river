using NuClear.OperationsTracking.API.UseCases;

namespace NuClear.AdvancedSearch.Replication.OperationsProcessing.Tests.Mocks
{
    public interface ITrackedUseCaseParser
    {
        TrackedUseCase Parse(byte[] data);
    }
}
