using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;

using NUnit.Framework;

namespace EntityDataModel.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxBuilderStoreModelTests : EdmxBuilderBaseFixture
    {
        [Test]
        public void ShouldExposeEntities()
        {
            var config = NewContext("Library").StoreModel(NewModel(
                NewEntity("Book"), 
                NewEntity("Author").EntitySetName("Authors")));

            var model = BuildStoreModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));
            Assert.That(model.Container.EntitySets, Has.Count.EqualTo(2));
            Assert.That(model.FindEntitySet("Book"), Is.Not.Null);
            Assert.That(model.FindEntitySet("Authors"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeEntityAccordingToSchema()
        {
            var config = NewContext("Library").StoreModel(NewModel(NewEntity("Catalog.Book").EntitySetName("Books")));

            var model = BuildStoreModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));
            
            var entitySet = model.FindEntitySet("Books");
            Assert.That(entitySet, Is.Not.Null);
            Assert.That(entitySet.Name, Is.EqualTo("Books"));
            Assert.That(entitySet.Schema, Is.EqualTo("Catalog"));
            Assert.That(entitySet.Table, Is.EqualTo("Book"));
        }

        [Test]
        public void ShouldExposeEntityTypes()
        {
            var config = NewContext("Library").StoreModel(NewModel(NewEntity("Book"), NewEntity("Author")));

            var model = BuildStoreModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));
            Assert.That(model.EntityTypes, Has.Count.EqualTo(2));
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Store.Book"), Is.Not.Null.And.InstanceOf<EntityType>());
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Store.Author"), Is.Not.Null.And.InstanceOf<EntityType>());
        }

        [Test]
        public void ShouldExposeEntityProperties()
        {
            var config = NewContext("Context").StoreModel(NewModel(
                    EntityElement.Config
                        .Name("Entity").IdentifyBy("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte).NotNull())
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                        ));

            var model = BuildStoreModel(config);
            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));

            var entityType = model.FindDeclaredType("AdvancedSearch.Context.Store.Entity");
            Assert.NotNull(entityType);

            Assert.AreEqual(2, entityType.DeclaredProperties.Count);
            Assert.That(entityType.FindProperty("Id"), Is.Not.Null.And.Matches(Property.OfType(PrimitiveTypeKind.Byte)));
            Assert.That(entityType.FindProperty("Name"), Is.Not.Null.And.Matches(Property.OfType(PrimitiveTypeKind.String)));
        }

        [Test]
        public void ShouldExposeEntityKeys()
        {
            var config = NewContext("Context").StoreModel(NewModel(
                    EntityElement.Config
                        .Name("Entity").IdentifyBy("Id", "Name")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte).NotNull())
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String).NotNull())
                        ));

            var model = BuildStoreModel(config);
            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));

            var entityType = model.FindDeclaredType("AdvancedSearch.Context.Store.Entity");
            Assert.NotNull(entityType);

            Assert.AreEqual(2, entityType.DeclaredProperties.Count);
            Assert.That(entityType.FindProperty("Id"), Is.Not.Null.And.Matches(Property.IsKey()));
            Assert.That(entityType.FindProperty("Name"), Is.Not.Null.And.Matches(Property.IsKey()));
        }

        [Test]
        public void ShouldSupportNullable()
        {
            var config = NewContext("Context").StoreModel(NewModel(
                    EntityElement.Config
                        .Name("Entity").IdentifyBy("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte).NotNull())
                        .Property(EntityPropertyElement.Config.Name("NonNullable").OfType(EntityPropertyType.String).NotNull())
                        .Property(EntityPropertyElement.Config.Name("Nullable").OfType(EntityPropertyType.String))
                        ));

            var model = BuildStoreModel(config);
            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));

            var entityType = model.FindDeclaredType("AdvancedSearch.Context.Store.Entity");
            Assert.NotNull(entityType);

            Assert.That(entityType.FindProperty("Nullable"), Is.Not.Null.And.Matches<EdmProperty>(x => x.Nullable));
            Assert.That(entityType.FindProperty("NonNullable"), Is.Not.Null.And.Not.Matches<EdmProperty>(x => x.Nullable));
        }

        [Test]
        public void ShouldSupportAllPrimitiveTypes()
        {
            var primitiveTypes = Enum.GetValues(typeof(EntityPropertyType)).OfType<EntityPropertyType>().Except(new[] { EntityPropertyType.Enum }).ToArray();

            var element = EntityElement.Config.Name("Entity").IdentifyBy("PropertyOfInt32");
            foreach (var propertyType in primitiveTypes)
            {
                element.Property(NewProperty("PropertyOf" + propertyType.ToString("G")).OfType(propertyType).NotNull());
            }

            var model = BuildStoreModel(NewContext("Context").StoreModel(NewModel(element)));
            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));

            var entity = model.FindDeclaredType("AdvancedSearch.Context.Store.Entity");
            Assert.NotNull(entity);
            Assert.AreEqual(primitiveTypes.Length, entity.DeclaredProperties.Count());
        }

        [Test, ExpectedException(typeof(NotSupportedException), ExpectedMessage = "enum property", MatchType = MessageMatch.Contains)]
        public void ShouldNotSupportEnumType()
        {
            var config = NewContext("Context").StoreModel(NewModel(
                    NewEntity("Person")
                    .Property(EntityPropertyElement.Config
                        .Name("Gender")
                        .UsingEnum("GenderEnum")
                        .WithMember("Male", 1)
                        .WithMember("Female", 2)
                        )
                    ));

            BuildStoreModel(config);
        }
    }
}