using System.Collections.Generic;
using System.Linq;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model
{
    public static class ErmToFactsTypeMap
    {
        private static readonly Dictionary<IEntityType, IEntityType> ErmToFactsTypeMapping
            = new[]
              {
                  CreateTypeMapping<EntityTypeAppointment, EntityTypeActivity>(),
                  CreateTypeMapping<EntityTypePhonecall, EntityTypeActivity>(),
                  CreateTypeMapping<EntityTypeTask, EntityTypeActivity>(),
                  CreateTypeMapping<EntityTypeLetter, EntityTypeActivity>(),
              }.ToDictionary(pair => pair.Key, pair => pair.Value);

        public static IEntityType MapErmToFacts(this IEntityType entityType)
        {
            IEntityType mappedEntityType;
            if (ErmToFactsTypeMapping.TryGetValue(entityType, out mappedEntityType))
            {
                return mappedEntityType;
            }

            return entityType;
        }

        private static KeyValuePair<IEntityType, IEntityType> CreateTypeMapping<TFrom, TTo>()
            where TFrom : IdentityBase<TFrom>, IEntityType, new()
            where TTo : IdentityBase<TTo>, IEntityType, new()
        {
            return new KeyValuePair<IEntityType, IEntityType>(IdentityBase<TFrom>.Instance, IdentityBase<TTo>.Instance);
        }
    }
}