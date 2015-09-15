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
                    EntityElement.Config.Name(EntityName.CategoryGroup).EntitySetName("CategoryGroups")
                        .HasKey("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String)),
                    EntityElement.Config.Name(EntityName.Project).EntitySetName("Projects")
                        .HasKey("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                        .Relation(EntityRelationElement.Config.Name("Categories")
                            .DirectTo(
                                EntityElement.Config.Name(EntityName.ProjectCategory).EntitySetName("ProjectCategories")
                                    .HasKey("CategoryId")
                                    .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                    .Property(EntityPropertyElement.Config.Name("Level").OfType(ElementaryTypeKind.Int32))
                            ).AsMany().AsContainment()
                        )
                        .Relation(EntityRelationElement.Config.Name("Territories")
                            .DirectTo(
                                EntityElement.Config.Name(EntityName.ProjectTerritory)
                                    .HasKey("Id")
                                    .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                            ).AsMany().AsContainment()
                        )
                        .Relation(EntityRelationElement.Config.Name("Firms")
                            .DirectTo(
                                EntityElement.Config.Name(EntityName.Firm).EntitySetName("Firms")
                                    .HasKey("Id")
                                    .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                    .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(ElementaryTypeKind.DateTimeOffset))
                                    .Property(EntityPropertyElement.Config.Name("LastDisqualifiedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                    .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                    .Property(EntityPropertyElement.Config.Name("LastActivityOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                    .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(ElementaryTypeKind.Boolean))
                                    .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(ElementaryTypeKind.Boolean))
                                    .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(ElementaryTypeKind.Int32))
                                    .Relation(EntityRelationElement.Config.Name("Balances")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.FirmBalance)
                                                .HasKey("AccountId")
                                                .Property(EntityPropertyElement.Config.Name("AccountId").OfType(ElementaryTypeKind.Int64))
                                                .Property(EntityPropertyElement.Config.Name("Balance").OfType(ElementaryTypeKind.Decimal))
                                        ).AsMany())
                                    .Relation(EntityRelationElement.Config.Name("Categories")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.FirmCategory)
                                                .HasKey("CategoryId")
                                                .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                                .Property(EntityPropertyElement.Config.Name("AdvertisersShare").OfType(ElementaryTypeKind.Double).Nullable())
                                                .Property(EntityPropertyElement.Config.Name("FirmCount").OfType(ElementaryTypeKind.Int64).Nullable())
                                                .Property(EntityPropertyElement.Config.Name("Hits").OfType(ElementaryTypeKind.Int64).Nullable())
                                                .Property(EntityPropertyElement.Config.Name("Shows").OfType(ElementaryTypeKind.Int64).Nullable())
                                        ).AsMany())
                                    .Relation(EntityRelationElement.Config.Name("CategoryGroup").DirectTo(EntityElement.Config.Name(EntityName.CategoryGroup)).AsOne())
                                    .Relation(EntityRelationElement.Config.Name("Client")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.Client)
                                                .HasKey("Id")
                                                .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                                .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                                .Relation(EntityRelationElement.Config.Name("CategoryGroup").DirectTo(EntityElement.Config.Name(EntityName.CategoryGroup)).AsOne())
                                                .Relation(
                                                    EntityRelationElement.Config.Name("Contacts")
                                                        .DirectTo(
                                                            EntityElement.Config.Name(EntityName.ClientContact)
                                                                .HasKey("ContactId")
                                                                .Property(EntityPropertyElement.Config.Name("ContactId").OfType(ElementaryTypeKind.Int64))
                                                                .Property(EntityPropertyElement.Config.Name("Role").OfType<EnumTypeElement>(EnumTypeElement.Config.Name(EnumName.ContactRole)))
                                                        )
                                                        .AsMany()
                                                )
                                        )
                                        .AsOneOptionally())
                                    .Property(EntityPropertyElement.Config.Name("OwnerId").OfType(ElementaryTypeKind.Int64))
                                    .Relation(EntityRelationElement.Config.Name("Territory").DirectTo(EntityElement.Config.Name(EntityName.ProjectTerritory)).AsOne())
                            )
                            .AsMany().AsContainment()
                        )
            );

            private static readonly StructuralModelElementBuilder StoreModel =
                StructuralModelElement.Config.Elements(
                    EntityElement.Config.Name(TableName.CategoryGroup)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Property(EntityPropertyElement.Config.Name("Rate").OfType(ElementaryTypeKind.Single)),
                    EntityElement.Config.Name(TableName.Project)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String)),
                    EntityElement.Config.Name(TableName.ProjectCategory)
                                 .HasKey("ProjectId", "CategoryId")
                                 .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Property(EntityPropertyElement.Config.Name("Level").OfType(ElementaryTypeKind.Int32))
                                 .Property(EntityPropertyElement.Config.Name("ParentId").OfType(ElementaryTypeKind.Int64).Nullable())
                                 .Relation(EntityRelationElement.Config.Name("ProjectId").DirectTo(EntityElement.Config.Name(TableName.Project)).AsOne()),
                    EntityElement.Config.Name(TableName.ProjectTerritory)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Relation(EntityRelationElement.Config.Name("ProjectId").DirectTo(EntityElement.Config.Name(TableName.Project)).AsOne()),
                    EntityElement.Config.Name(TableName.Client)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Relation(EntityRelationElement.Config.Name("CategoryGroupId").DirectTo(EntityElement.Config.Name(TableName.CategoryGroup)).AsOne()),
                    EntityElement.Config.Name(TableName.ClientContact)
                                 .HasKey("ClientId", "ContactId")
                                 .Property(EntityPropertyElement.Config.Name("ContactId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Role").OfType(ElementaryTypeKind.Int32))
                                 .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOne()),
                    EntityElement.Config.Name(ViewName.Firm)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                 .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(ElementaryTypeKind.DateTimeOffset))
                                 .Property(EntityPropertyElement.Config.Name("LastDisqualifiedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("LastActivityOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(ElementaryTypeKind.Boolean))
                                 .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(ElementaryTypeKind.Boolean))
                                 .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(ElementaryTypeKind.Int32))
                                 .Property(EntityPropertyElement.Config.Name("OwnerId").OfType(ElementaryTypeKind.Int64))
                                 .Relation(EntityRelationElement.Config.Name("CategoryGroupId").DirectTo(EntityElement.Config.Name(TableName.CategoryGroup)).AsOne())
                                 .Relation(EntityRelationElement.Config.Name("TerritoryId").DirectTo(EntityElement.Config.Name(TableName.ProjectTerritory)).AsOne())
                                 .Relation(EntityRelationElement.Config.Name("ProjectId").DirectTo(EntityElement.Config.Name(TableName.Project)).AsOne())
                                 .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOneOptionally()),
                    EntityElement.Config.Name(TableName.FirmBalance)
                                 .HasKey("FirmId", "AccountId")
                                 .Property(EntityPropertyElement.Config.Name("AccountId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Balance").OfType(ElementaryTypeKind.Decimal))
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(ViewName.Firm)).AsOne()),
                    EntityElement.Config.Name(TableName.FirmCategory)
                                 .HasKey("FirmId", "CategoryId")
                                 .Property(EntityPropertyElement.Config.Name("CategoryId").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("AdvertisersShare").OfType(ElementaryTypeKind.Double).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("FirmCount").OfType(ElementaryTypeKind.Int64).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("Hits").OfType(ElementaryTypeKind.Int64).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("Shows").OfType(ElementaryTypeKind.Int64).Nullable())
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(ViewName.Firm)).AsOne()));

            public static readonly BoundedContextElement Context =
                BoundedContextElement.Config.Name("CustomerIntelligence")
                    .ConceptualModel(ConceptualModel)
                    .StoreModel(StoreModel)
                    .Map(EntityName.CategoryGroup, TableName.CategoryGroup)
                    .Map(EntityName.Project, TableName.Project)
                    .Map(EntityName.ProjectCategory, TableName.ProjectCategory)
                    .Map(EntityName.ProjectTerritory, TableName.ProjectTerritory)
                    .Map(EntityName.Firm, ViewName.Firm)
                    .Map(EntityName.FirmBalance, TableName.FirmBalance)
                    .Map(EntityName.FirmCategory, TableName.FirmCategory)
                    .Map(EntityName.Client, TableName.Client)
                    .Map(EntityName.ClientContact, TableName.ClientContact);

            private static class EnumName
            {
                public const string ContactRole = "ContactRole";
            }

            private static class EntityName
            {
                public const string CategoryGroup = "CategoryGroup";
                public const string Project = "Project";
                public const string ProjectCategory = "ProjectCategory";
                public const string ProjectTerritory = "Territory";
                public const string Client = "Client";
                public const string ClientContact = "ClientContact";
                public const string Firm = "Firm";
                public const string FirmBalance = "FirmBalance";
                public const string FirmCategory = "FirmCategory";
            }

            private static class TableName
            {
                public const string CategoryGroup = TableSchema + "." + "CategoryGroup";
                public const string Project = TableSchema + "." + "Project";
                public const string ProjectCategory = TableSchema + "." + "ProjectCategory";
                public const string ProjectTerritory = TableSchema + "." + "Territory";
                public const string FirmBalance = TableSchema + "." + "FirmBalance";
                public const string FirmCategory = TableSchema + "." + "FirmCategory";
                public const string Client = TableSchema + "." + "Client";
                public const string ClientContact = TableSchema + "." + "ClientContact";

                private const string TableSchema = "CustomerIntelligence";
            }

            private static class ViewName
            {
                public const string Firm = ViewSchema + "." + "FirmView";

                private const string ViewSchema = "CustomerIntelligence";
            }
        }
    }
}