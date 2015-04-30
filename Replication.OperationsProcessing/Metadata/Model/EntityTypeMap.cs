using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Model.EntityTypes;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes;

using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using Erm = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model
{
    public static class EntityTypeMap
    {
        private static readonly Dictionary<IEntityType, Type> ErmTypeMap
            = new[]
              {
                  CreateMapping<EntityTypeAccount, Erm.Account>(),
                  CreateMapping<EntityTypeCategoryFirmAddress, Erm.CategoryFirmAddress>(),
                  CreateMapping<EntityTypeCategoryOrganizationUnit, Erm.CategoryOrganizationUnit>(),
                  CreateMapping<EntityTypeClient, Erm.Client>(),
                  CreateMapping<EntityTypeContact, Erm.Contact>(),
                  CreateMapping<EntityTypeFirm, Erm.Firm>(),
                  CreateMapping<EntityTypeFirmAddress, Erm.FirmAddress>(),
                  CreateMapping<EntityTypeFirmContact, Erm.FirmContact>(),
                  CreateMapping<EntityTypeLegalPerson, Erm.LegalPerson>(),
                  CreateMapping<EntityTypeOrder, Erm.Order>(),
              }.ToDictionary(pair => pair.Key, pair => pair.Value);

        private static readonly Dictionary<IEntityType, Type> CustomerIntelligenceTypeMap
            = new[]
              {
                  CreateMapping<EntityTypeClient, CI.Client>(),
                  CreateMapping<EntityTypeContact, CI.Contact>(),
                  CreateMapping<EntityTypeFirm, CI.Firm>(),
                  CreateMapping<EntityTypeFirmBalance, CI.FirmBalance>(),
                  CreateMapping<EntityTypeFirmCategory, CI.FirmCategory>(),
                  CreateMapping<EntityTypeFirmCategoryGroup, CI.FirmCategoryGroup>(),
              }.ToDictionary(pair => pair.Key, pair => pair.Value);

        public static void Initialize()
        {
            EntityTypeMappingRegistry.Initialize(Enumerable.Empty<IEntityType>(), Enumerable.Empty<Type>());
            EntityTypeMappingRegistry.AddMappings(ErmTypeMap); // FIXME {a.rechkalov, 30.04.2015}: Пока зарегистрировать можно только типы из контекста Erm или типы из CI - выбирай, что важнее в данный момент
        }

        private static KeyValuePair<IEntityType, Type> CreateMapping<TEntityType, TAggregateType>()
            where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
        {
            return new KeyValuePair<IEntityType, Type>(IdentityBase<TEntityType>.Instance, typeof(TAggregateType));
        }
    }
}