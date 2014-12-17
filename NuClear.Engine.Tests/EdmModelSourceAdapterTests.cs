using System;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Engine.Tests
{
    [TestFixture]
    internal class EdmModelSourceAdapterTests
    {
        [Test]
        public void ShouldExposeNamespace()
        {
            var modelSource = Adapt(BoundedContextElement.Config.Name("ContextName"));
            
            Assert.NotNull(modelSource);
            Assert.AreEqual("AdvancedSearch.ContextName", modelSource.Namespace);
            Assert.AreEqual(0, modelSource.Entities.Count);
        }

        [Test, ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "context name was not specified")]
        public void ShouldFailIfNoContextName()
        {
            Adapt(BoundedContextElement.Config);
        }

        [Test]
        public void ShouldExposeEntities()
        {
            var config = BoundedContextElement.Config
                .Name("Library")
                .Elements(
                    EntityElement.Config.Name("Book"),
                    EntityElement.Config.Name("Author")
                    );

            var modelSource = Adapt(config);
            
            Assert.NotNull(modelSource);
            Assert.AreEqual(2, modelSource.Entities.Count);
            Assert.That(modelSource.Entities.FirstOrDefault(Entity.ByName("Book")), Is.Not.Null);
            Assert.That(modelSource.Entities.FirstOrDefault(Entity.ByName("Author")), Is.Not.Null);
        }

        [Test, ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "entity name was not specified")]
        public void ShouldFailIfNoEntityName()
        {
            Adapt(BoundedContextElement.Config.Name("Context").Elements(EntityElement.Config));
        }

        [Test]
        public void ShouldExposeEntityProperties()
        {
            var config = BoundedContextElement.Config
                .Name("Context")
                .Elements(
                    EntityElement.Config
                        .Name("Entity")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte))
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                        .IdentifyBy("Id"));

            var modelSource = Adapt(config);
            
            Assert.NotNull(modelSource);
            Assert.AreEqual(1, modelSource.Entities.Count);

            var entity = modelSource.Entities.Single();
            Assert.AreEqual(2, entity.Properties.Count);
            Assert.That(entity.Properties.FirstOrDefault(Entity.Property.ByName("Id")), Is.Not.Null);
            Assert.That(entity.Properties.FirstOrDefault(Entity.Property.ByName("Name")), Is.Not.Null);
        }

        [Test, ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "property name was not specified")]
        public void ShouldFailIfNoPropertyName()
        {
            Adapt(BoundedContextElement.Config.Name("Context").Elements(
                EntityElement.Config
                    .Name("Entity")
                    .Property(EntityPropertyElement.Config)
                ));
        }

        [Test]
        public void ShouldSupportAllPropertyTypes()
        {
            var element = EntityElement.Config.Name("Entity");

            var typeCodes = Enum.GetValues(typeof(EntityPropertyType)).OfType<EntityPropertyType>().ToArray();
            foreach (var typeCode in typeCodes)
            {
                element.Property(EntityPropertyElement.Config.Name("PropertyOf" + typeCode.ToString("G")).OfType(typeCode));
            }

            var config = BoundedContextElement.Config.Name("Context").Elements(element);

            var modelSource = Adapt(config);
            Assert.NotNull(modelSource);

            var entity = modelSource.Entities.Single();
            Assert.AreEqual(typeCodes.Length, entity.Properties.Count);
        }

        [Test]
        public void ShouldDistinguishEntityTypes()
        {
            var config = BoundedContextElement.Config
                .Name("Context")
                .Elements(
                    EntityElement.Config.Name("EntityType")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte))
                        .IdentifyBy("Id"),
                    EntityElement.Config.Name("ComplexType")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte))
                    );

            var modelSource = Adapt(config);

            Assert.NotNull(modelSource);
            Assert.AreEqual(2, modelSource.Entities.Count);
            Assert.That(modelSource.Entities.FirstOrDefault(Entity.ByName("EntityType")), Is.Not.Null.And.Matches(Entity.OfEntityType));
            Assert.That(modelSource.Entities.FirstOrDefault(Entity.ByName("ComplexType")), Is.Not.Null.And.Matches(Entity.OfComplexType));
        }

        #region Utils

        private static IEdmModelSource Adapt(BoundedContextElementBuilder config)
        {
            return new AdvancedSearchMetadataSourceAdapter(config);
        }

        public static class Entity
        {
            public static Func<EdmEntityInfo, bool> ByName(string name)
            {
                return x => x.Name == name;
            }

            public static Predicate<EdmEntityInfo> OfEntityType { get { return x => x.HasKey; } }

            public static Predicate<EdmEntityInfo> OfComplexType { get { return x => !x.HasKey; } }

            public static class Property
            {
                public static Func<EdmEntityPropertyInfo, bool> ByName(string name)
                {
                    return x => x.Name == name;
                }
            }
        }

        #endregion
    }
}
