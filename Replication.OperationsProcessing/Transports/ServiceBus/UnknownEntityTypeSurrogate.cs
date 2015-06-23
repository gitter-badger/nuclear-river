using NuClear.Model.Common.Entities;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf.Surrogates;

using ProtoBuf;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    public sealed class UnknownEntityTypeSurrogate : IdentitySurrogate
    {
        [ProtoConverter]
        public static IEntityType From(UnknownEntityTypeSurrogate value)
        {
            if (value == null)
            {
                return null;
            }

            IEntityType entityType;
            if (EntityType.Instance.TryParse(value.Id, out entityType))
            {
                return entityType;
            }

            return new UnknownEntityType().SetId(value.Id);
        }

        [ProtoConverter]
        public static UnknownEntityTypeSurrogate To(IEntityType value)
        {
            return value == null ? null : new UnknownEntityTypeSurrogate { Id = value.Id };
        }
    }
}