// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    partial class AdvancedSearchMetadataSource
    {
        private static class BusinessDirectory
        {
            private static readonly StructuralModelElementBuilder ConceptualModel =
                StructuralModelElement.Config.Elements(
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
                    EntityElement.Config.Name(EntityName.CategoryGroup).EntitySetName("CategoryGroups")
                        .HasKey("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String)));

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
                    EntityElement.Config.Name(TableName.Category)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String)),
                    EntityElement.Config.Name(TableName.CategoryGroup)
                                 .HasKey("Id")
                                 .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int64))
                                 .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String)));

            public static readonly BoundedContextElement Context =
                BoundedContextElement.Config.Name("BusinessDirectory")
                    .ConceptualModel(ConceptualModel)
                    .StoreModel(StoreModel)
                    .Map(EntityName.OrganizationUnit, TableName.OrganizationUnit)
                    .Map(EntityName.Territory, TableName.Territory)
                    .Map(EntityName.Category, TableName.Category)
                    .Map(EntityName.CategoryGroup, TableName.CategoryGroup);

            private static class EntityName
            {
                public const string OrganizationUnit = "OrganizationUnit";
                public const string Territory = "Territory";
                public const string Category = "Category";
                public const string CategoryGroup = "CategoryGroup";
            }

            private static class TableName
            {
                public const string OrganizationUnit = TableSchema + "." + "OrganizationUnit";
                public const string Territory = TableSchema + "." + "Territory";
                public const string Category = TableSchema + "." + "Category";
                public const string CategoryGroup = TableSchema + "." + "CategoryGroup";

                private const string TableSchema = "BusinessDirectory";
            }
        }
    }
}