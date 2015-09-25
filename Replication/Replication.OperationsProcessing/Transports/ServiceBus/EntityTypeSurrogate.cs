
using NuClear.Model.Common.Entities;

using ProtoBuf;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    internal sealed class EntityTypeSurrogate
    {
        public int Id { get; set; }

        [ProtoConverter]
        public static IEntityType From(EntityTypeSurrogate value)
        {
            if (value == null)
            {
                return null;
            }

            IEntityType entityType;
            if (!EntityType.Instance.TryParse(value.Id, out entityType))
            {
                entityType = new UnknownEntityType().SetId(value.Id);
            }

            return entityType;
        }

        [ProtoConverter]
        public static EntityTypeSurrogate To(IEntityType value)
        {
            if (value == null)
            {
                return null;
            }

            var surrogate = SurrogateFactory<EntityTypeSurrogate>.Factory();
            surrogate.Id = value.Id;

            return surrogate;
        }
    }
}