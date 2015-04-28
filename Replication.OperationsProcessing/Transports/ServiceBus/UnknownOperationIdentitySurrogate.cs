using NuClear.Model.Common.Operations.Identity;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;

using ProtoBuf;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class UnknownOperationIdentitySurrogate : IdentitySurrogate
    {
        [ProtoConverter]
        public static IOperationIdentity From(UnknownOperationIdentitySurrogate value)
        {
            return value == null ? null : new UnknownOperationIdentity().SetId(value.Id);
        }

        [ProtoConverter]
        public static UnknownOperationIdentitySurrogate To(IOperationIdentity value)
        {
            return value == null ? null : new UnknownOperationIdentitySurrogate { Id = value.Id };
        }
    }
}