using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.Model.EntityTypes;
using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.Replication.OperationsProcessing.Metadata.Model.Context;
using NuClear.Replication.OperationsProcessing.Metadata.Model.EntityTypes;

using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using Erm = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Erm;
using Facts = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

namespace NuClear.Replication.OperationsProcessing.Metadata.Model
{
    public static class EntityTypeMap
    {
        private static readonly Dictionary<IEntityType, Type> ErmTypeMap
            = new[]
                {
                    CreateMapping<EntityTypeAccount, Erm.Account>(),
                    CreateMapping<EntityTypeBranchOfficeOrganizationUnit, Erm.BranchOfficeOrganizationUnit>(),
                    CreateMapping<EntityTypeCategory, Erm.Category>(),
                    CreateMapping<EntityTypeCategoryFirmAddress, Erm.CategoryFirmAddress>(),
                    CreateMapping<EntityTypeCategoryGroup, Erm.CategoryGroup>(),
                    CreateMapping<EntityTypeCategoryOrganizationUnit, Erm.CategoryOrganizationUnit>(),
                    CreateMapping<EntityTypeClient, Erm.Client>(),
                    CreateMapping<EntityTypeContact, Erm.Contact>(),
                    CreateMapping<EntityTypeFirm, Erm.Firm>(),
                    CreateMapping<EntityTypeFirmAddress, Erm.FirmAddress>(),
                    CreateMapping<EntityTypeFirmContact, Erm.FirmContact>(),
                    CreateMapping<EntityTypeLegalPerson, Erm.LegalPerson>(),
                    CreateMapping<EntityTypeOrder, Erm.Order>(),
                    CreateMapping<EntityTypeProject, Erm.Project>(),
                    CreateMapping<EntityTypeTerritory, Erm.Territory>(),
                }.ToDictionary(pair => pair.Key, pair => pair.Value);

        private static readonly Dictionary<IEntityType, Type> FactsTypeMap
            = new[]
                {
                    CreateMapping<EntityTypeAccount, Facts.Account>(),
                    CreateMapping<EntityTypeBranchOfficeOrganizationUnit, Facts.BranchOfficeOrganizationUnit>(),
                    CreateMapping<EntityTypeCategory, Facts.Category>(),
                    CreateMapping<EntityTypeCategoryFirmAddress, Facts.CategoryFirmAddress>(),
                    CreateMapping<EntityTypeCategoryGroup, Facts.CategoryGroup>(),
                    CreateMapping<EntityTypeCategoryOrganizationUnit, Facts.CategoryOrganizationUnit>(),
                    CreateMapping<EntityTypeClient, Facts.Client>(),
                    CreateMapping<EntityTypeContact, Facts.Contact>(),
                    CreateMapping<EntityTypeFirm, Facts.Firm>(),
                    CreateMapping<EntityTypeFirmAddress, Facts.FirmAddress>(),
                    CreateMapping<EntityTypeFirmContact, Facts.FirmContact>(),
                    CreateMapping<EntityTypeLegalPerson, Facts.LegalPerson>(),
                    CreateMapping<EntityTypeOrder, Facts.Order>(),
                    CreateMapping<EntityTypeProject, Facts.Project>(),
                    CreateMapping<EntityTypeTerritory, Facts.Territory>(),
                }.ToDictionary(pair => pair.Key, pair => pair.Value);

        private static readonly Dictionary<IEntityType, Type> CustomerIntelligenceTypeMap
            = new[]
                {
                    CreateMapping<EntityTypeCategoryGroup, CI.CategoryGroup>(),
                    CreateMapping<EntityTypeClient, CI.Client>(),
                    CreateMapping<EntityTypeContact, CI.ClientContact>(),
                    CreateMapping<EntityTypeFirm, CI.Firm>(),
                    CreateMapping<EntityTypeFirmBalance, CI.FirmBalance>(),
                    CreateMapping<EntityTypeFirmCategory, CI.FirmCategory>(),
                    CreateMapping<EntityTypeProject, CI.Project>(),
                    CreateMapping<EntityTypeProjectCategory, CI.ProjectCategory>(),
                    CreateMapping<EntityTypeTerritory, CI.Territory>(),
                }.ToDictionary(pair => pair.Key, pair => pair.Value);

        public static void Initialize()
        {
            EntityTypeMappingRegistry.Initialize<ErmContext>(Enumerable.Empty<IEntityType>(), Enumerable.Empty<Type>());
            EntityTypeMappingRegistry.AddMappings<ErmContext>(ErmTypeMap);

            EntityTypeMappingRegistry.Initialize<CustomerIntelligenceContext>(Enumerable.Empty<IEntityType>(), Enumerable.Empty<Type>());
            EntityTypeMappingRegistry.AddMappings<CustomerIntelligenceContext>(CustomerIntelligenceTypeMap);

            EntityTypeMappingRegistry.Initialize<FactsContext>(Enumerable.Empty<IEntityType>(), Enumerable.Empty<Type>());
            EntityTypeMappingRegistry.AddMappings<FactsContext>(FactsTypeMap);
        }

        private static KeyValuePair<IEntityType, Type> CreateMapping<TEntityType, TAggregateType>()
            where TEntityType : IdentityBase<TEntityType>, IEntityType, new()
        {
            return new KeyValuePair<IEntityType, Type>(IdentityBase<TEntityType>.Instance, typeof(TAggregateType));
        }
    }
}