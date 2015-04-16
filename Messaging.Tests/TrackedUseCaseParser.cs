using System.IO;

using NuClear.AdvancedSearch.Messaging.ServiceBus;
using NuClear.OperationsTracking.API.UseCases;

using ProtoBuf.Meta;

namespace NuClear.AdvancedSearch.Messaging.Tests
{
    public sealed class TrackedUseCaseParser : ITrackedUseCaseParser
    {
        private readonly TypeModel _typeModel = AdvancedSearchTypeModel.CreateTypeModel();

        public TrackedUseCase Parse(byte[] data)
        {
            var stream = new MemoryStream(data);
            var trackedUseCase = (TrackedUseCase)_typeModel.Deserialize(stream, null, typeof(TrackedUseCase));
            trackedUseCase.Tracker.SynchronizeAuxiliaryData();
            trackedUseCase.Tracker.Complete();

            return trackedUseCase;
        }
    }
}