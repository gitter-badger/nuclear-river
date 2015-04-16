using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;

using ProtoBuf.Meta;

namespace NuClear.AdvancedSearch.Messaging.ServiceBus
{
    public sealed class TrackedUseCaseConfigurator : IRuntimeTypeModelConfigurator
    {
        public RuntimeTypeModel Configure(RuntimeTypeModel typeModel)
        {
            typeModel.Add(typeof(UnknownOperationIdentitySurrogate), false)
                     .Add(1, "Id");
            typeModel.Add(typeof(IOperationIdentity), false).SetSurrogate(typeof(UnknownOperationIdentitySurrogate));

            typeModel.Add(typeof(UnknownEntityTypeSurrogate), false)
                     .Add(1, "Id");
            typeModel.Add(typeof(IEntityType), false).SetSurrogate(typeof(UnknownEntityTypeSurrogate));

            return typeModel;
        }
    }
}
