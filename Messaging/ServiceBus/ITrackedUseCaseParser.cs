using System.IO;

using NuClear.OperationsTracking.API.UseCases;

namespace NuClear.AdvancedSearch.Messaging.ServiceBus
{
    public interface ITrackedUseCaseParser
    {
        TrackedUseCase Parse(Stream stream);
    }
}
