// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    partial class AdvancedSearchMetadataSource
    {
        private static class CustomerIntelligence
        {
            private static readonly StructuralModelElementBuilder ConceptualModel =
                StructuralModelElement.Config
                .Types<EnumTypeElement>(
                    EnumTypeElement.Config.Name(EnumName.ContactRole)
                        .Member("Employee", 1)
                        .Member("InfluenceDecisions", 2)
                        .Member("MakingDecisions", 3)
                )
                .Elements(
                    EntityElement.Config.Name(EntityName.Firm).EntitySetName("Firms")
                        .HasKey("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                        .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(ElementaryTypeKind.DateTimeOffset))
                        .Property(EntityPropertyElement.Config.Name("LastDisqualifiedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                        .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                        .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(ElementaryTypeKind.Boolean))
                        .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(ElementaryTypeKind.Boolean))
                        .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(ElementaryTypeKind.Int32))
                        .Relation(EntityRelationElement.Config.Name("Balances")
                            .DirectTo(
                                EntityElement.Config.Name(EntityName.FirmBalance)
                                    .HasKey("AccountId", "FirmId")
                                    .Property(EntityPropertyElement.Config.Name("AccountId").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("FirmId").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("Balance").OfType(ElementaryTypeKind.Decimal))
                            ).AsMany())
                        .Relation(EntityRelationElement.Config.Name("Client")
                            .DirectTo(
                                EntityElement.Config.Name(EntityName.Client)
                                    .HasKey("Id")
                                    .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                    .Property(EntityPropertyElement.Config.Name("CategoryGroupId").OfType(ElementaryTypeKind.Int64))
                                    .Relation(
                                        EntityRelationElement.Config.Name("Contacts")
                                            .DirectTo(
                                                EntityElement.Config.Name(EntityName.Contact)
                                                    .HasKey("Id")
                                                    .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                                    .Property(EntityPropertyElement.Config.Name("Role").OfType<EnumTypeElement>(EnumTypeElement.Config.Name(EnumName.ContactRole)))
                                                    .Property(EntityPropertyElement.Config.Name("IsFired").OfType(ElementaryTypeKind.Boolean))
                                            )
                                            .AsMany()
                                    )
                            )
                            .AsOneOptionally())
                        .Property(EntityPropertyElement.Config.Name("CategoryGroupId").OfType(ElementaryTypeKind.Int64))
                        .Property(EntityPropertyElement.Config.Name("OrganizationUnitId").OfType(ElementaryTypeKind.Int64))
                        .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(ElementaryTypeKind.Int64))
                        .Relation(EntityRelationElement.Config.Name("Categories")
                            .DirectTo(
                                EntityElement.Config.Name(EntityName.FirmCategory)
                                    .HasKey("CategoryId", "FirmId")
                                    .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("FirmId").OfType(ElementaryTypeKind.Int64))
                            ).AsMany())
                        .Relation(EntityRelationElement.Config.Name("CategoryGroups")
                            .DirectTo(
                                EntityElement.Config.Name(EntityName.FirmCategoryGroup)
                                    .HasKey("CategoryGroupId", "FirmId")
                                    .Property(EntityPropertyElement.Config.Name("CategoryGroupId").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("FirmId").OfType(ElementaryTypeKind.Int64))
                            ).AsMany()));

            private static readonly StructuralModelElementBuilder StoreModel =
                StructuralModelElement.Config.Elements(
                    EntityElement.Config.Name(TableName.Client)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Property(EntityPropertyElement.Config.Name("CategoryGroupId").OfType(ElementaryTypeKind.Int64)),
                    EntityElement.Config.Name(TableName.Contact)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Role").OfType(ElementaryTypeKind.Int32))
                                 .Property(EntityPropertyElement.Config.Name("IsFired").OfType(ElementaryTypeKind.Boolean))
                                 .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOne()),
                    EntityElement.Config.Name(TableName.Firm)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(ElementaryTypeKind.DateTimeOffset))
                                 .Property(EntityPropertyElement.Config.Name("LastDisqualifiedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(ElementaryTypeKind.Boolean))
                                 .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(ElementaryTypeKind.Boolean))
                                 .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(ElementaryTypeKind.Int32))
                                 .Property(EntityPropertyElement.Config.Name("CategoryGroupId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("OrganizationUnitId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(ElementaryTypeKind.Int64))
                                 .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOneOptionally()),
                    EntityElement.Config.Name(TableName.FirmBalance)
                                 .HasKey("FirmId", "AccountId")
                                 .Property(EntityPropertyElement.Config.Name("AccountId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Balance").OfType(ElementaryTypeKind.Decimal))
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(TableName.Firm)).AsOne()),
                    EntityElement.Config.Name(TableName.FirmCategory)
                                 .HasKey("FirmId", "CategoryId")
                                 .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(TableName.Firm)).AsOne()),
                    EntityElement.Config.Name(TableName.FirmCategoryGroup)
                                 .HasKey("FirmId", "CategoryGroupId")
                                 .Property(EntityPropertyElement.Config.Name("CategoryGroupId").OfType(ElementaryTypeKind.Int64))
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(TableName.Firm)).AsOne()));

            public static readonly BoundedContextElement Context =
                BoundedContextElement.Config.Name("CustomerIntelligence")
                    .ConceptualModel(ConceptualModel)
                    .StoreModel(StoreModel)
                    .Map(EntityName.Firm, TableName.Firm)
                    .Map(EntityName.FirmBalance, TableName.FirmBalance)
                    .Map(EntityName.Client, TableName.Client)
                    .Map(EntityName.Contact, TableName.Contact)
                    .Map(EntityName.FirmCategory, TableName.FirmCategory)
                    .Map(EntityName.FirmCategoryGroup, TableName.FirmCategoryGroup);

            private static class EnumName
            {
                public const string ContactRole = "ContactRole";
            }

            private static class EntityName
            {
                public const string Client = "Client";
                public const string Contact = "Contact";
                public const string Firm = "Firm";
                public const string FirmBalance = "FirmBalance";
                public const string FirmCategory = "FirmCategory";
                public const string FirmCategoryGroup = "FirmCategoryGroup";
            }

            private static class TableName
            {
                public const string Client = TableSchema + "." + "Client";
                public const string Contact = TableSchema + "." + "Contact";
                public const string Firm = TableSchema + "." + "Firm";
                public const string FirmBalance = TableSchema + "." + "FirmBalance";
                public const string FirmCategory = TableSchema + "." + "FirmCategories";
                public const string FirmCategoryGroup = TableSchema + "." + "FirmCategoryGroups";

                private const string TableSchema = "CustomerIntelligence";
            }
        }
    }
}