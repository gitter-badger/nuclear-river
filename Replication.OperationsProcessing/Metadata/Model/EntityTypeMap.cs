using System;

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
        private static readonly Action<EntityTypeMappingRegistryBuilder> ErmTypeMap
            = builder => builder
					.AddMapping<EntityTypeAppointment, Erm.Appointment>()
                    .AddMapping<EntityTypePhonecall, Erm.Phonecall>()
					.AddMapping<EntityTypeTask, Erm.Task>()
                    .AddMapping<EntityTypeLetter, Erm.Letter>()
                    .AddMapping<EntityTypeAccount, Erm.Account>()
                    .AddMapping<EntityTypeBranchOfficeOrganizationUnit, Erm.BranchOfficeOrganizationUnit>()
                    .AddMapping<EntityTypeCategory, Erm.Category>()
                    .AddMapping<EntityTypeCategoryFirmAddress, Erm.CategoryFirmAddress>()
                    .AddMapping<EntityTypeCategoryGroup, Erm.CategoryGroup>()
                    .AddMapping<EntityTypeCategoryOrganizationUnit, Erm.CategoryOrganizationUnit>()
                    .AddMapping<EntityTypeClient, Erm.Client>()
                    .AddMapping<EntityTypeContact, Erm.Contact>()
                    .AddMapping<EntityTypeFirm, Erm.Firm>()
                    .AddMapping<EntityTypeFirmAddress, Erm.FirmAddress>()
                    .AddMapping<EntityTypeFirmContact, Erm.FirmContact>()
                    .AddMapping<EntityTypeLegalPerson, Erm.LegalPerson>()
                    .AddMapping<EntityTypeOrder, Erm.Order>()
                    .AddMapping<EntityTypeProject, Erm.Project>()
                    .AddMapping<EntityTypeTerritory, Erm.Territory>();

		private static readonly Action<EntityTypeMappingRegistryBuilder> FactsTypeMap
            = builder => builder
                    .AddMapping<EntityTypeActivity, Facts.Activity>()
                    .AddMapping<EntityTypeAccount, Facts.Account>()
                    .AddMapping<EntityTypeBranchOfficeOrganizationUnit, Facts.BranchOfficeOrganizationUnit>()
                    .AddMapping<EntityTypeCategory, Facts.Category>()
                    .AddMapping<EntityTypeCategoryFirmAddress, Facts.CategoryFirmAddress>()
                    .AddMapping<EntityTypeCategoryGroup, Facts.CategoryGroup>()
                    .AddMapping<EntityTypeCategoryOrganizationUnit, Facts.CategoryOrganizationUnit>()
                    .AddMapping<EntityTypeClient, Facts.Client>()
                    .AddMapping<EntityTypeContact, Facts.Contact>()
                    .AddMapping<EntityTypeFirm, Facts.Firm>()
                    .AddMapping<EntityTypeFirmAddress, Facts.FirmAddress>()
                    .AddMapping<EntityTypeFirmContact, Facts.FirmContact>()
                    .AddMapping<EntityTypeLegalPerson, Facts.LegalPerson>()
                    .AddMapping<EntityTypeOrder, Facts.Order>()
                    .AddMapping<EntityTypeProject, Facts.Project>()
                    .AddMapping<EntityTypeTerritory, Facts.Territory>();

		private static readonly Action<EntityTypeMappingRegistryBuilder> CustomerIntelligenceTypeMap
            = builder => builder
                    .AddMapping<EntityTypeCategoryGroup, CI.CategoryGroup>()
                    .AddMapping<EntityTypeClient, CI.Client>()
                    .AddMapping<EntityTypeContact, CI.ClientContact>()
                    .AddMapping<EntityTypeFirm, CI.Firm>()
                    .AddMapping<EntityTypeFirmBalance, CI.FirmBalance>()
                    .AddMapping<EntityTypeFirmCategory, CI.FirmCategory>()
                    .AddMapping<EntityTypeProject, CI.Project>()
                    .AddMapping<EntityTypeProjectCategory, CI.ProjectCategory>()
                    .AddMapping<EntityTypeTerritory, CI.Territory>();

	    public static IEntityTypeMappingRegistry<ErmSubDomain> CreateErmContext()
	    {
		    var builder = new EntityTypeMappingRegistryBuilder();
		    ErmTypeMap.Invoke(builder);
		    return builder.Create<ErmSubDomain>();
	    }

	    public static IEntityTypeMappingRegistry<CustomerIntelligenceSubDomain> CreateCustomerIntelligenceContext()
		{
			var builder = new EntityTypeMappingRegistryBuilder();
			CustomerIntelligenceTypeMap.Invoke(builder);
			return builder.Create<CustomerIntelligenceSubDomain>();
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