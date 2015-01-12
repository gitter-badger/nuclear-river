using System;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;

using NUnit.Framework;

namespace EntityDataModel.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxBuilderConceptualModelTests : EdmxBuilderBaseFixture
    {
        [Test]
        public void ShouldExposeEdmEntities()
        {
            var config = NewContext("Library").ConceptualModel(NewModel(NewEntity("Book"), NewEntity("Author")));

            var model = BuildConceptualModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(ConceptualModel.IsValid));
            Assert.That(model.Container.EntitySets, Has.Count.EqualTo(2));
            Assert.That(model.Container.FindEntitySet("Book"), Is.Not.Null);
            Assert.That(model.Container.FindEntitySet("Author"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeEntityTypes()
        {
            var config = NewContext("Library").ConceptualModel(NewModel(NewEntity("Book"), NewEntity("Author")));

            var model = BuildConceptualModel(config);

            Assert.NotNull(model);
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Book"), Is.Not.Null.And.InstanceOf<EntityType>());
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Author"), Is.Not.Null.And.InstanceOf<EntityType>());
        }

        [Test]
        public void ShouldExposeEntityProperties()
        {
            var config = NewContext("Context").ConceptualModel(NewModel(
                    EntityElement.Config
                        .Name("Entity").IdentifyBy("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte).NotNull())
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                        ));

            var model = BuildConceptualModel(config);
            Assert.NotNull(model);

            var entityType = model.FindDeclaredType("AdvancedSearch.Context.Entity");
            Assert.NotNull(entityType);

            Assert.AreEqual(2, entityType.DeclaredProperties.Count);
            Assert.That(entityType.FindProperty("Id"), Is.Not.Null.And.Matches(Property.OfType(PrimitiveTypeKind.Byte)));
            Assert.That(entityType.FindProperty("Name"), Is.Not.Null.And.Matches(Property.OfType(PrimitiveTypeKind.String)));
        }

        [Test]
        public void ShouldSupportEnumType()
        {
            var config = NewContext("Context").ConceptualModel(NewModel(
                    NewEntity("Person")
                    .Property(EntityPropertyElement.Config
                        .Name("Gender")
                        .UsingEnum("GenderEnum")
                        .WithMember("Male", 1)
                        .WithMember("Female", 2)
                        )
                    ));

            var model = BuildConceptualModel(config);
            Assert.NotNull(model);

            var entity = model.FindDeclaredType("AdvancedSearch.Context.Person");
            Assert.NotNull(entity);
            Assert.That(entity.FindProperty("Gender"), Is.Not.Null.And.Matches(Property.Members("Male", "Female")));
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

            var model = BuildConceptualModel(NewContext("Context").ConceptualModel(NewModel(element)));
            Assert.NotNull(model);

            var entity = model.FindDeclaredType("AdvancedSearch.Context.Entity");
            Assert.NotNull(entity);
            Assert.AreEqual(primitiveTypes.Length, entity.DeclaredProperties.Count());
        }
    }
}
