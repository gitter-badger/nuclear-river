using NuClear.OperationsTracking.API.UseCases;

namespace NuClear.AdvancedSearch.Messaging.Tests
{
    public interface ITrackedUseCaseParser
    {
        TrackedUseCase Parse(byte[] data);
    }
}
