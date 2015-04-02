using System.IO;

using NuClear.OperationsTracking.API.UseCases;

namespace NuClear.AdvancedSearch.ServiceBus
{
    public interface ITrackedUseCaseParser
    {
        TrackedUseCase Parse(Stream stream);
    }
}
