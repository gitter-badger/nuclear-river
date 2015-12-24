using System;

using NuClear.ValidationRules.OperationsProcessing.Contexts;
using NuClear.ValidationRules.OperationsProcessing.Identities.EntityTypes;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;

using Erm = NuClear.ValidationRules.Domain.Model.Erm;
using Facts = NuClear.ValidationRules.Domain.Model.Facts;

namespace NuClear.ValidationRules.OperationsProcessing
{
    public static class EntityTypeMap
    {
        private static readonly Action<EntityTypeMappingRegistryBuilder> ErmTypeMap
            = builder => builder
                             .AddMapping<EntityTypeOrder, Erm::Order>();

        private static readonly Action<EntityTypeMappingRegistryBuilder> FactsTypeMap
            = builder => builder
                             .AddMapping<EntityTypeOrder, Facts::Order>();


        public static IEntityTypeMappingRegistry<ErmSubDomain> CreateErmContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            ErmTypeMap.Invoke(builder);
            return builder.Create<ErmSubDomain>();
        }

        public static IEntityTypeMappingRegistry<FactsSubDomain> CreateFactsContext()
        {
            var builder = new EntityTypeMappingRegistryBuilder();
            FactsTypeMap.Invoke(builder);
            return builder.Create<FactsSubDomain>();
        }
    }

    public static class EntityTypeMappingRegistryBuilderExtensions
    {
        public static EntityTypeMappingRegistryBuilder AddMapping<TEntityType, TAggregateType>(this EntityTypeMappingRegistryBuilder registryBuilder)
            where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
        {
            return registryBuilder.AddMapping(IdentityBase<TEntityType>.Instance, typeof(TAggregateType));
        }
    }
}