using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;

using NUnit.Framework;

namespace EntityDataModel.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxBuilderModelTests : EdmxBuilderBaseFixture
    {
        private const string DemoConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\AdvancedSearch.mdf";
        private static readonly IDbConnectionFactory DefaultFactory = new LocalDbConnectionFactory("mssqllocaldb");

        private readonly BoundedContextElement _customerIntelligence =
            BoundedContextElement.Config
                .Name("CustomerIntelligence")
                .ConceptualModel(
                    StructuralModelElement.Config.Elements(
                        EntityElement.Config
                            .Name("Firm")//.EntitySetName("Firms")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                            .Property(EntityPropertyElement.Config.Name("OrganizationUnitId").OfType(EntityPropertyType.Int64))
                            .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(EntityPropertyType.Int64))
                            .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("LastQualifiedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(EntityPropertyType.Boolean))
                            .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(EntityPropertyType.Boolean))
                            .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Byte))
                            .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(EntityPropertyType.Int32))
//                            .Relation(EntityRelationElement.Config
//                                .Name("Categories")
//                                .DirectTo(
//                                    EntityElement.Config
//                                        .Name("Category")
//                                        .IdentifyBy("Id")
//                                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
//                                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
//                                        .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Byte))
//                                )
//                                .AsMany())
                    )
                )
                .StoreModel(
                    StructuralModelElement.Config.Elements(
                         EntityElement.Config
                            .Name("dbo.Firm")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                            .Property(EntityPropertyElement.Config.Name("OrganizationUnitId").OfType(EntityPropertyType.Int64))
                            .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(EntityPropertyType.Int64))
                            .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("LastQualifiedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(EntityPropertyType.DateTime))
                            .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(EntityPropertyType.Boolean))
                            .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(EntityPropertyType.Boolean))
                            .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Byte))
                            .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(EntityPropertyType.Int32))
                            .Property(EntityPropertyElement.Config.Name("ClientId").OfType(EntityPropertyType.Int64))//,
//                         EntityElement.Config
//                            .Name("dbo.Category")
//                            .IdentifyBy("Id")
//                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
//                            .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
//                            .Property(EntityPropertyElement.Config.Name("Level").OfType(EntityPropertyType.Int32).NotNull())
//                            .Property(EntityPropertyElement.Config.Name("ParentId").OfType(EntityPropertyType.Int64))
                   )
                )
                ;

        [Test]
        public void ShouldQueryData()
        {
            var model = BuildModel(_customerIntelligence).Compile();

            Assert.That(model, Is.Not.Null);

            var connection = DefaultFactory.CreateConnection(DemoConnectionString);
            using (var context = model.CreateObjectContext<ObjectContext>(connection))
            {
                var records = new ObjectQuery<DbDataRecord>("SELECT firm.Id FROM Firm as firm", context);
                foreach (var record in records)
                {
                    Debug.WriteLine(record.GetString(record.GetOrdinal("Id")));
                }
            }
        }
    }
}