using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;

using ProtoBuf.Meta;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class TrackedUseCaseConfigurator : IRuntimeTypeModelConfigurator
    {
        public TrackedUseCaseConfigurator(IOperationIdentityRegistry registry)
        {
            SurrogateFactory<OperationIdentitySurrogate>.Factory = () => new OperationIdentitySurrogate(registry);
            SurrogateFactory<EntityTypeSurrogate>.Factory = () => new EntityTypeSurrogate();
        }

        public RuntimeTypeModel Configure(RuntimeTypeModel typeModel)
        {
            typeModel.Add(typeof(OperationIdentitySurrogate), false)
                     .Add(1, "Id").SetFactory(SurrogateFactory<OperationIdentitySurrogate>.Factory.Method);

            typeModel.Add(typeof(IOperationIdentity), false).SetSurrogate(typeof(OperationIdentitySurrogate));

            typeModel.Add(typeof(EntityTypeSurrogate), false)
                     .Add(1, "Id").SetFactory(SurrogateFactory<EntityTypeSurrogate>.Factory.Method);
            typeModel.Add(typeof(IEntityType), false).SetSurrogate(typeof(EntityTypeSurrogate));

            return typeModel;
        }
    }
}
