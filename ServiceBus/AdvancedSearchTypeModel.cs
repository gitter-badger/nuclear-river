using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;

using ProtoBuf.Meta;

namespace NuClear.AdvancedSearch.ServiceBus
{
    public sealed class AdvancedSearchTypeModel
    {
        public static TypeModel CreateTypeModel()
        {
            var typeModel = ProtoBufTypeModelForTrackedUseCaseConfigurator.Configure();
            var compiledTypeModel = typeModel.Compile();
            return compiledTypeModel;
        }
    }
}
