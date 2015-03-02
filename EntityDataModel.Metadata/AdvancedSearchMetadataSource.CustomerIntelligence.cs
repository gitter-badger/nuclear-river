// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    partial class AdvancedSearchMetadataSource
    {
        private static readonly StructuralModelElementBuilder ConceptualModel =
            StructuralModelElement.Config
            .Types<EnumTypeElement>(
                EnumTypeElement.Config.Name(EnumName.ContactRole)
                    .Member("Employee", 200000)
                    .Member("InfluenceDecisions", 200001)
                    .Member("MakingDecisions", 200002),
                EnumTypeElement.Config.Name(EnumName.CategoryGroup)
                    .Member("Percent120", 1)
                    .Member("Percent110", 2)
                    .Member("Normal", 3)
                    .Member("Percent90", 4)
                    .Member("Percent50", 5)
            )
            .Elements(
                EntityElement.Config.Name(EntityName.OrganizationUnit).EntitySetName("OrganizationUnits")
                    .HasKey("Id")
                    .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String)),
                EntityElement.Config.Name(EntityName.Territory).EntitySetName("Territories")
                    .HasKey("Id")
                    .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                    .Relation(EntityRelationElement.Config.Name("OrganizationUnit").DirectTo(EntityElement.Config.Name(EntityName.OrganizationUnit)).AsOne()),
                EntityElement.Config.Name(EntityName.Category).EntitySetName("Categories")
                    .HasKey("Id")
                    .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                    .Property(EntityPropertyElement.Config.Name("Level").OfType(ElementaryTypeKind.Int32)),
                EntityElement.Config.Name(EntityName.Firm).EntitySetName("Firms")
                    .HasKey("Id")
                    .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                    .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                    .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(ElementaryTypeKind.DateTimeOffset))
                    .Property(EntityPropertyElement.Config.Name("LastDisqualifiedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                    .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(ElementaryTypeKind.DateTimeOffset).Nullable())
                    .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(ElementaryTypeKind.Boolean))
                    .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(ElementaryTypeKind.Boolean))
                    .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(ElementaryTypeKind.Int32))
                    .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType<EnumTypeElement>(EnumTypeElement.Config.Name(EnumName.CategoryGroup)))
                    .Relation(EntityRelationElement.Config.Name("OrganizationUnit").DirectTo(EntityElement.Config.Name(EntityName.OrganizationUnit)).AsOne())
                    .Relation(EntityRelationElement.Config.Name("Territory").DirectTo(EntityElement.Config.Name(EntityName.Territory)).AsOne())
                    .Relation(EntityRelationElement.Config.Name("Categories1")
                        .DirectTo(
                            EntityElement.Config.Name(EntityName.FirmCategory1)
                                .HasKey("Id")
                                .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.String))
                                .Relation(EntityRelationElement.Config.Name("Category").DirectTo(EntityElement.Config.Name(EntityName.Category)).AsOne())
                        )
                        .AsMany())
                    .Relation(EntityRelationElement.Config.Name("Categories2")
                        .DirectTo(
                            EntityElement.Config.Name(EntityName.FirmCategory2)
                                .HasKey("Id")
                                .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.String))
                                .Relation(EntityRelationElement.Config.Name("Category").DirectTo(EntityElement.Config.Name(EntityName.Category)).AsOne())
                        )
                        .AsMany())
                    .Relation(EntityRelationElement.Config.Name("Categories3")
                        .DirectTo(
                            EntityElement.Config.Name(EntityName.FirmCategory3)
                                .HasKey("Id")
                                .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType<EnumTypeElement>(EnumTypeElement.Config.Name(EnumName.CategoryGroup)))
                                .Relation(EntityRelationElement.Config.Name("Category").DirectTo(EntityElement.Config.Name(EntityName.Category)).AsOne())
                        )
                        .AsMany())
                    .Relation(EntityRelationElement.Config.Name("Client")
                        .DirectTo(
                            EntityElement.Config.Name(EntityName.Client)
                                .HasKey("Id")
                                .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                                .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType<EnumTypeElement>(EnumTypeElement.Config.Name(EnumName.CategoryGroup)))
                                .Relation(
                                    EntityRelationElement.Config.Name("Accounts")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.Account)
                                                .HasKey("Id")
                                                .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                                .Property(EntityPropertyElement.Config.Name("Balance").OfType(ElementaryTypeKind.Decimal))
                                        )
                                        .AsMany()
                                )
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
                        .AsOneOptionally()));

        private static readonly StructuralModelElementBuilder StoreModel =
            StructuralModelElement.Config.Elements(
                EntityElement.Config.Name(TableName.OrganizationUnit)
                             .HasKey("Id")
                             .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                             .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String)),
                EntityElement.Config.Name(TableName.Territory)
                             .HasKey("Id")
                             .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                             .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                             .Relation(EntityRelationElement.Config.Name("OrganizationUnitId").DirectTo(EntityElement.Config.Name(TableName.OrganizationUnit)).AsOne()),
                EntityElement.Config.Name(TableName.Client)
                             .HasKey("Id")
                             .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                             .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                             .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(ElementaryTypeKind.Int32)),
                EntityElement.Config.Name(TableName.Account)
                             .HasKey("Id")
                             .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                             .Property(EntityPropertyElement.Config.Name("Balance").OfType(ElementaryTypeKind.Decimal))
                             .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOne()),
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
                             .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(ElementaryTypeKind.Boolean))
                             .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(ElementaryTypeKind.Boolean))
                             .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(ElementaryTypeKind.Int32))
                             .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(ElementaryTypeKind.Int32))
                             .Relation(EntityRelationElement.Config.Name("OrganizationUnitId").DirectTo(EntityElement.Config.Name(TableName.OrganizationUnit)).AsOne())
                             .Relation(EntityRelationElement.Config.Name("TerritoryId").DirectTo(EntityElement.Config.Name(TableName.Territory)).AsOne())
                             .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOneOptionally()),
                EntityElement.Config.Name(TableName.FirmCategory1)
                             .HasKey("Id")
                             .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.String))
                             .Relation(EntityRelationElement.Config.Name("CategoryId").DirectTo(EntityElement.Config.Name(TableName.Category)).AsOne())
                             .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(TableName.Firm)).AsOne()),
                EntityElement.Config.Name(TableName.FirmCategory2)
                             .HasKey("Id")
                             .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.String))
                             .Relation(EntityRelationElement.Config.Name("CategoryId").DirectTo(EntityElement.Config.Name(TableName.Category)).AsOne())
                             .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(TableName.Firm)).AsOne()),
                EntityElement.Config.Name(TableName.FirmCategory3)
                             .HasKey("Id")
                             .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                             .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(ElementaryTypeKind.Int32))
                             .Relation(EntityRelationElement.Config.Name("CategoryId").DirectTo(EntityElement.Config.Name(TableName.Category)).AsOne())
                             .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(TableName.Firm)).AsOne()),
                EntityElement.Config.Name(TableName.Category)
                             .HasKey("Id")
                             .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                             .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                             .Property(EntityPropertyElement.Config.Name("Level").OfType(ElementaryTypeKind.Int32)));

        private static readonly BoundedContextElement CustomerIntelligenceContext =
            BoundedContextElement.Config.Name("CustomerIntelligence")
                .ConceptualModel(ConceptualModel)
                .StoreModel(StoreModel)
                .Map(EntityName.Firm, TableName.Firm)
                .Map(EntityName.FirmCategory1, TableName.FirmCategory1)
                .Map(EntityName.FirmCategory2, TableName.FirmCategory2)
                .Map(EntityName.FirmCategory3, TableName.FirmCategory3)
                .Map(EntityName.Category, TableName.Category)
                .Map(EntityName.Client, TableName.Client)
                .Map(EntityName.Account, TableName.Account)
                .Map(EntityName.Contact, TableName.Contact)
                .Map(EntityName.OrganizationUnit, TableName.OrganizationUnit)
                .Map(EntityName.Territory, TableName.Territory);

        private static class EnumName
        {
            public const string ContactRole = "ContactRole";
            public const string CategoryGroup = "CategoryGroup";
        }

        private static class EntityName
        {
            public const string Firm = "Firm";
            public const string FirmCategory1 = "FirmCategory1";
            public const string FirmCategory2 = "FirmCategory2";
            public const string FirmCategory3 = "FirmCategory3";
            public const string Category = "Category";
            public const string Client = "Client";
            public const string Account = "Account";
            public const string Contact = "Contact";
            public const string OrganizationUnit = "OrganizationUnit";
            public const string Territory = "Territory";
        }

        private static class TableName
        {
            public const string Firm = TableSchema + "." + "Firm";
            public const string FirmCategory1 = TableSchema + "." + "FirmCategory1";
            public const string FirmCategory2 = TableSchema + "." + "FirmCategory2";
            public const string FirmCategory3 = TableSchema + "." + "FirmCategory3";
            public const string Category = TableSchema + "." + "Category";
            public const string Client = TableSchema + "." + "Client";
            public const string Account = TableSchema + "." + "Account";
            public const string Contact = TableSchema + "." + "Contact";
            public const string OrganizationUnit = TableSchema + "." + "OrganizationUnit";
            public const string Territory = TableSchema + "." + "Territory";

            private const string TableSchema = "CustomerIntelligence";
        }
    }
}