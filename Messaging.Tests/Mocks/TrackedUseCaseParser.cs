using System.IO;
using System.Linq;

using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;
using NuClear.OperationsTracking.API.UseCases;
using NuClear.Replication.OperationsProcessing.Transports.ServiceBus;

using ProtoBuf.Meta;

namespace NuClear.AdvancedSearch.Messaging.Tests.Mocks
{
    public sealed class TrackedUseCaseParser : ITrackedUseCaseParser
    {
        private readonly RuntimeTypeModel _typeModel;

        public TrackedUseCaseParser()
        {
            var configurators = new IRuntimeTypeModelConfigurator[]
                                {
                                    new ProtoBufTypeModelForTrackedUseCaseConfigurator(),
                                    new TrackedUseCaseConfigurator()
                                };

            _typeModel = configurators.Aggregate(TypeModel.Create(), (x, y) => y.Configure(x));
        }

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