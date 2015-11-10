using NuClear.Model.Common.Entities;
using NuClear.OperationsLogging.Transports.ServiceBus.Serialization.ProtoBuf.Surrogates;

using ProtoBuf;

namespace NuClear.Replication.OperationsProcessing.Transports.ServiceBus
{
    internal sealed class EntityTypeSurrogate
    {
        private readonly IEntityTypeMappingRegistry<ISubDomain> _entityTypeMappingRegistry;

        public EntityTypeSurrogate(IEntityTypeMappingRegistry<ISubDomain> entityTypeMappingRegistry)
        {
            _entityTypeMappingRegistry = entityTypeMappingRegistry;
        }

        public int Id { get; set; }

        [ProtoConverter]
        public static IEntityType From(EntityTypeSurrogate value)
        {
            if (value == null)
            {
                return null;
            }

            if (value.Id == EntityTypeNone.Instance.Id)
            {
                return EntityTypeNone.Instance;
            }

            if (value.Id == EntityTypeAll.Instance.Id)
            {
                return EntityTypeAll.Instance;
            }

            IEntityType entityType;
            if (!value._entityTypeMappingRegistry.TryParse(value.Id, out entityType))
            {
                entityType = new UnknownEntityType(value.Id);
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