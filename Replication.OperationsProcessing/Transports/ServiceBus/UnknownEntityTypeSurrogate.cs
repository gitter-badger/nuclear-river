using NuClear.Model.Common.Entities;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf;

using ProtoBuf;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class UnknownEntityTypeSurrogate : IdentitySurrogate
    {
        [ProtoConverter]
        public static IEntityType From(UnknownEntityTypeSurrogate value)
        {
            return value == null ? null : new UnknownEntityType().SetId(value.Id);
        }

        [ProtoConverter]
        public static UnknownEntityTypeSurrogate To(IEntityType value)
        {
            return value == null ? null : new UnknownEntityTypeSurrogate { Id = value.Id };
        }
    }
}