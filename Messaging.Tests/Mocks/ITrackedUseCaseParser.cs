using NuClear.OperationsTracking.API.UseCases;

namespace NuClear.AdvancedSearch.Messaging.Tests.Mocks
{
    public interface ITrackedUseCaseParser
    {
        TrackedUseCase Parse(byte[] data);
    }
}
