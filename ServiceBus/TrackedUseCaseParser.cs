using System.IO;

using NuClear.OperationsTracking.API.UseCases;

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