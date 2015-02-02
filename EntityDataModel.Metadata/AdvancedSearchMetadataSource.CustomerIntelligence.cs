// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    partial class AdvancedSearchMetadataSource
    {
        private static readonly StructuralModelElementBuilder ConceptualModel =
            StructuralModelElement.Config.Elements(
                EntityElement.Config.Name(EntityName.Firm).EntitySetName("Firms")
                    .HasKey("Id")
                    .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                    .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                    .Property(EntityPropertyElement.Config.Name("OrganizationUnitId").OfType(EntityPropertyType.Int64))
                    .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(EntityPropertyType.Int64))
                    .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(EntityPropertyType.DateTimeOffset))
                    .Property(EntityPropertyElement.Config.Name("LastDisqualifiedOn").OfType(EntityPropertyType.DateTimeOffset).Nullable())
                    .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(EntityPropertyType.DateTimeOffset).Nullable())
                    .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(EntityPropertyType.Boolean))
                    .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(EntityPropertyType.Boolean))
                    .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Int64).Nullable())
                    .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(EntityPropertyType.Int32))
                    .Relation(EntityRelationElement.Config.Name("Categories")
                        .DirectTo(
                            EntityElement.Config.Name(EntityName.Category)
                                .HasKey("Id")
                                .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                                .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                                .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Int64))
                        )
                        .AsMany())
                    .Relation(EntityRelationElement.Config.Name("Client")
                        .DirectTo(
                            EntityElement.Config.Name(EntityName.Client)
                                .HasKey("Id")
                                .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                                .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Int64))
                                .Relation(
                                    EntityRelationElement.Config.Name("Accounts")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.Account)
                                                .HasKey("Id")
                                                .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                                                .Property(EntityPropertyElement.Config.Name("Balance").OfType(EntityPropertyType.Decimal))
                                        )
                                        .AsMany()
                                )
                                .Relation(
                                    EntityRelationElement.Config.Name("Contacts")
                                        .DirectTo(
                                            EntityElement.Config.Name(EntityName.Contact)
                                                .HasKey("Id")
                                                .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                                                .Property(EntityPropertyElement.Config.Name("Role")
                                                    .UsingEnum("ContactRole")
                                                    .WithMember("Employee", 200000)
                                                    .WithMember("InfluenceDecisions", 200001)
                                                    .WithMember("MakingDecisions", 200002)
                                                    )
                                                .Property(EntityPropertyElement.Config.Name("IsFired").OfType(EntityPropertyType.Boolean))
                                        )
                                        .AsMany()
                                )
                        )
                        .AsOneOptionally()));

        private static readonly StructuralModelElementBuilder StoreModel =
            StructuralModelElement.Config
                .Elements(
                    EntityElement.Config.Name(TableName.Client)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                                 .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Int64)),
                    EntityElement.Config.Name(TableName.Account)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Balance").OfType(EntityPropertyType.Decimal))
                                 .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOne()),
                    EntityElement.Config.Name(TableName.Contact)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Role").OfType(EntityPropertyType.Int32))
                                 .Property(EntityPropertyElement.Config.Name("IsFired").OfType(EntityPropertyType.Boolean))
                                 .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOne()),
                    EntityElement.Config.Name(TableName.Firm)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                                 .Property(EntityPropertyElement.Config.Name("OrganizationUnitId").OfType(EntityPropertyType.Int64))
                                 .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(EntityPropertyType.Int64))
                                 .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(EntityPropertyType.DateTimeOffset))
                                 .Property(EntityPropertyElement.Config.Name("LastDisqualifiedOn").OfType(EntityPropertyType.DateTimeOffset).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(EntityPropertyType.DateTimeOffset).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(EntityPropertyType.Boolean))
                                 .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(EntityPropertyType.Boolean))
                                 .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Int64).Nullable())
                                 .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(EntityPropertyType.Int32))
                                 .Relation(EntityRelationElement.Config.Name("ClientId").DirectTo(EntityElement.Config.Name(TableName.Client)).AsOneOptionally()),
                    EntityElement.Config.Name(TableName.Category)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                                 .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Level").OfType(EntityPropertyType.Int32))
                                 .Property(EntityPropertyElement.Config.Name("ParentId").OfType(EntityPropertyType.Int64))
                                 .Relation(EntityRelationElement.Config.Name("FirmId").DirectTo(EntityElement.Config.Name(TableName.Firm)).AsOne()));

        private static readonly BoundedContextElement CustomerIntelligenceContext =
            BoundedContextElement.Config.Name("CustomerIntelligence")
                .ConceptualModel(ConceptualModel)
                .StoreModel(StoreModel)
                .Map(EntityName.Firm, TableName.Firm)
                .Map(EntityName.Category, TableName.Category)
                .Map(EntityName.Client, TableName.Client)
                .Map(EntityName.Account, TableName.Account)
                .Map(EntityName.Contact, TableName.Contact);

        private static class EntityName
        {
            public const string Firm = "Firm";
            public const string Category = "Category";
            public const string Client = "Client";
            public const string Account = "Account";
            public const string Contact = "Contact";
        }

        private static class TableName
        {
            public const string Firm = TableSchema + "." + "Firm";
            public const string Category = TableSchema + "." + "Category";
            public const string Client = TableSchema + "." + "Client";
            public const string Account = TableSchema + "." + "Account";
            public const string Contact = TableSchema + "." + "Contact";

            private const string TableSchema = "CustomerIntelligence";
        }
    }
}