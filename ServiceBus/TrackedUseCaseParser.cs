using System.IO;

using NuClear.AdvancedSearch.ServiceBus.Contracts;
using NuClear.AdvancedSearch.ServiceBus.Contracts.DTO;

using ProtoBuf.Meta;

namespace NuClear.AdvancedSearch.ServiceBus
{
    public sealed class TrackedUseCaseParser : ITrackedUseCaseParser
    {
        private readonly TypeModel _typeModel = AdvancedSearchTypeModel.CreateTypeModel();

        public TrackedUseCase Parse(Stream stream)
        {
            var trackedUseCase = (TrackedUseCase)_typeModel.Deserialize(stream, null, typeof(TrackedUseCase));
            return trackedUseCase;
        }
    }
}