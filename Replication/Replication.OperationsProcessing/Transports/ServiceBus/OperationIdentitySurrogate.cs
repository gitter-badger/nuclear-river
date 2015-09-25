using NuClear.Model.Common.Operations.Identity;

using ProtoBuf;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    internal sealed class OperationIdentitySurrogate
    {
        private readonly IOperationIdentityRegistry _registry;

        public int Id { get; set; }

        public OperationIdentitySurrogate(IOperationIdentityRegistry registry)
        {
            _registry = registry;
        }

        [ProtoConverter]
        public static IOperationIdentity From(OperationIdentitySurrogate value)
        {
            if (value == null)
            {
                return null;
            }

            IOperationIdentity operationIdentity;
            if (!value._registry.TryGetIdentity(value.Id, out operationIdentity))
            {
                operationIdentity = new UnknownOperationIdentity().SetId(value.Id);
            }

            return operationIdentity;
        }

        [ProtoConverter]
        public static OperationIdentitySurrogate To(IOperationIdentity value)
        {
            if (value == null)
            {
                return null;
            }

            var surrogate = SurrogateFactory<OperationIdentitySurrogate>.Factory();
            surrogate.Id = value.Id;

            return surrogate;
        }
    }
}