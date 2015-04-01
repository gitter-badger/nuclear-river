using System.IO;

using NuClear.AdvancedSearch.ServiceBus.Contracts.DTO;

namespace NuClear.AdvancedSearch.ServiceBus.Contracts
{
    public interface ITrackedUseCaseParser
    {
        TrackedUseCase Parse(Stream stream);
    }
}
