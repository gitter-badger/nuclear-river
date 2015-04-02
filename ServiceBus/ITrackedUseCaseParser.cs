using System.IO;

using NuClear.AdvancedSearch.ServiceBus.Contracts.DTO;

namespace NuClear.AdvancedSearch.ServiceBus
{
    public interface ITrackedUseCaseParser
    {
        TrackedUseCase Parse(Stream stream);
    }
}
