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
        [Test, ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "context name was not specified")]
        public void ShouldFailIfNoContextName()
        {
            Adapt(BoundedContextElement.Config);
        }

        [Test]
        public void ShouldExposeNamespace()
        {
            var modelSource = Adapt(BoundedContextElement.Config.Name("ContextName"));
            
            Assert.NotNull(modelSource);
            Assert.AreEqual("AdvancedSearch.ContextName", modelSource.Namespace);
            Assert.AreEqual(0, modelSource.Entities.Count);
        }

        [Test, ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "entity name was not specified")]
        public void ShouldFailIfNoEntityName()
        {
            Adapt(BoundedContextElement.Config.Name("Context").Elements(EntityElement.Config));
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

        [Test]
        public void ShouldSupportEnumType()
        {
            var config = BoundedContextElement.Config
                .Name("Context")
                .Elements(
                    EntityElement.Config
                    .Name("Person")
                    .Property(EntityPropertyElement.Config.Name("Gender")
                        .UsingEnum()
                        .WithMember("Male", 1)
                        .WithMember("Female", 2)
                        )
                    );

            var modelSource = Adapt(config);
            Assert.NotNull(modelSource);

            var entity = modelSource.Entities.Single();
            Assert.AreEqual(1, entity.Properties.Count);
            Assert.That(entity.Properties.FirstOrDefault(Entity.Property.ByName("Gender")), Is.Not.Null.And.Matches(Entity.Property.Members("Male", "Female")));
        }

        [Test]
        public void ShouldSupportAllPrimitiveTypes()
        {
            var element = EntityElement.Config.Name("Entity");

            var propertyTypes = Enum.GetValues(typeof(EntityPropertyType)).OfType<EntityPropertyType>().Except(new[] {EntityPropertyType.Enum}).ToArray();
            foreach (var propertyType in propertyTypes)
            {
                element.Property(EntityPropertyElement.Config.Name("PropertyOf" + propertyType.ToString("G")).OfType(propertyType));
            }

            var config = BoundedContextElement.Config.Name("Context").Elements(element);

            var modelSource = Adapt(config);
            Assert.NotNull(modelSource);

            var entity = modelSource.Entities.Single();
            Assert.AreEqual(propertyTypes.Length, entity.Properties.Count);
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

        [Test]
        public void ShouldExposeRelatedTarget()
        {
            var config = BoundedContextElement.Config
                .Name("Context")
                .Elements(
                    EntityElement.Config.Name("Entity")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte))
                        .IdentifyBy("Id")
                        .Relation(EntityRelationElement.Config
                            .Name("Categories")
                            .DirectTo(
                                EntityElement.Config.Name("RelatedEntity")
                                    .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                            )
                            .AsOne())
                    );

            var modelSource = Adapt(config);

            Assert.NotNull(modelSource);
            Assert.AreEqual(2, modelSource.Entities.Count);
            Assert.That(modelSource.Entities.FirstOrDefault(Entity.ByName("Entity")), Is.Not.Null);
            Assert.That(modelSource.Entities.FirstOrDefault(Entity.ByName("RelatedEntity")), Is.Not.Null);
        }

        #region Utils

        private static IEdmModelSource Adapt(BoundedContextElementBuilder config)
        {
            return new AdvancedSearchMetadataSourceAdapter(config);
        }

        public static class Entity
        {
            public static Func<EdmEntityType, bool> ByName(string name)
            {
                return x => x.Name == name;
            }

            public static Predicate<EdmEntityType> OfEntityType { get { return x => x.HasKey; } }

            public static Predicate<EdmEntityType> OfComplexType { get { return x => !x.HasKey; } }

            public static class Property
            {
                public static Func<EdmEntityPropertyInfo, bool> ByName(string name)
                {
                    return x => x.Name == name;
                }

                public static Predicate<EdmEntityPropertyInfo> Members(params string[] names)
                {
                    return x => names.OrderBy(_ => _).SequenceEqual(((EdmEnumType)x.TypeReference.Type).Members.Select(m => m.Key).OrderBy(_ => _));
                }
            }
        }

        #endregion
    }
}
