using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.OData.Edm;

using Moq;

using NuClear.AdvancedSearch.Engine.Tests;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Tests
{
    [TestFixture]
    internal class EdmModelBuilderTests
    {
        [Test, ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "context name was not specified")]
        public void ShouldFailIfNoContextName()
        {
            BuildModel(BoundedContextElement.Config);
        }

        [Test]
        public void ShouldExposeNamespace()
        {
            var model = BuildModel(BoundedContextElement.Config.Name("ContextName"));
            
            Assert.NotNull(model);
            Assert.That(model.DeclaredNamespaces.SingleOrDefault(), Is.Not.Null.And.EqualTo("AdvancedSearch.ContextName"));
            Assert.That(model.EntityContainer, Is.Not.Null);
            Assert.That(model.EntityContainer.Elements, Has.Count.EqualTo(0));
        }

        [Test, ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "entity name was not specified")]
        public void ShouldFailIfNoEntityName()
        {
            BuildModel(BoundedContextElement.Config.Name("Context").Elements(EntityElement.Config));
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

            var model = BuildModel(config);
            
            Assert.NotNull(model);
            Assert.That(model.EntityContainer.Elements, Has.Count.EqualTo(2));
            Assert.That(model.EntityContainer.FindEntitySet("Book"), Is.Not.Null);
            Assert.That(model.EntityContainer.FindEntitySet("Author"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeEntityTypes()
        {
            var config = BoundedContextElement.Config
                .Name("Library")
                .Elements(
                    EntityElement.Config.Name("Book"),
                    EntityElement.Config.Name("Author")
                    );

            var model = BuildModel(config);
            
            Assert.NotNull(model);
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Book"), Is.Not.Null.And.InstanceOf<IEdmEntityType>());
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Author"), Is.Not.Null.And.InstanceOf<IEdmEntityType>());
        }

        [Test, ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "property name was not specified")]
        public void ShouldFailIfNoPropertyName()
        {
            BuildModel(BoundedContextElement.Config.Name("Context").Elements(
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

            var model = BuildModel(config);
            Assert.NotNull(model);

            var entityType = model.FindDeclaredType("AdvancedSearch.Context.Entity") as IEdmEntityType;
            Assert.NotNull(entityType);

            Assert.AreEqual(2, entityType.DeclaredProperties.Count());
            Assert.That(entityType.FindProperty("Id"), Is.Not.Null);
            Assert.That(entityType.FindProperty("Name"), Is.Not.Null);
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
                        .UsingEnum("GenderEnum")
                        .WithMember("Male", 1)
                        .WithMember("Female", 2)
                        )
                    );

            var model = BuildModel(config);
            Assert.NotNull(model);

            var entity = model.FindDeclaredType("AdvancedSearch.Context.Person") as IEdmEntityType;
            Assert.NotNull(entity);
            Assert.That(entity.FindProperty("Gender"), Is.Not.Null.And.Matches(Entity.Property.Members("Male", "Female")));
        }

        [Test]
        public void ShouldSupportAllPrimitiveTypes()
        {
            var primitiveTypes = Enum.GetValues(typeof(EntityPropertyType)).OfType<EntityPropertyType>().Except(new[] { EntityPropertyType.Enum }).ToArray();

            var config = BoundedContextElement.Config.Name("Context");
            var element = EntityElement.Config.Name("Entity");
            foreach (var propertyType in primitiveTypes)
            {
                element.Property(EntityPropertyElement.Config.Name("PropertyOf" + propertyType.ToString("G")).OfType(propertyType));
            }

            var model = BuildModel(config.Elements(element));
            Assert.NotNull(model);

            var entity = model.FindDeclaredType("AdvancedSearch.Context.Entity") as IEdmEntityType;
            Assert.NotNull(entity);
            Assert.AreEqual(primitiveTypes.Length, entity.DeclaredProperties.Count());
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

            var model = BuildValidModel(config);

//            Assert.NotNull(modelSource);
//            Assert.AreEqual(2, modelSource.Entities.Count);
//            Assert.That(modelSource.Entities.FirstOrDefault(Entity.ByName("EntityType")), Is.Not.Null.And.Matches(Entity.OfEntityType));
//            Assert.That(modelSource.Entities.FirstOrDefault(Entity.ByName("ComplexType")), Is.Not.Null.And.Matches(Entity.OfComplexType));
        }

        [Test]
        public void ShouldExposeRelatedTargetType()
        {
            var config = BoundedContextElement.Config
                .Name("Context")
                .Elements(
                    EntityElement.Config.Name("Entity")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte).NotNull()).IdentifyBy("Id")
                        .Relation(EntityRelationElement.Config
                            .Name("Categories")
                            .DirectTo(
                                EntityElement.Config.Name("RelatedEntity")
                                    .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                            )
                            .AsOne())
                    );

            var model = BuildValidModel(config);

            var complexType = model.FindDeclaredType("AdvancedSearch.Context.RelatedEntity") as IEdmComplexType;
            Assert.NotNull(complexType);
        }

        #region Utils

        private static BoundedContextElement ProcessContext(BoundedContextElement context)
        {
            var source = new Mock<IMetadataSource>();
            source.Setup(x => x.Kind).Returns(new AdvancedSearchIdentity());
            source.Setup(x => x.Metadata).Returns(new Dictionary<Uri, IMetadataElement> { { IdBuilder.For<AdvancedSearchIdentity>(), context } });

            var sources = new IMetadataSource[] { source.Object };
            var processors = new IMetadataProcessor[0];

            var provider = new MetadataProvider(sources, processors);

            return provider.Metadata.Metadata.Values.OfType<BoundedContextElement>().FirstOrDefault();
        }

        private static IEdmModel BuildModel(BoundedContextElementBuilder config)
        {
            var context = ProcessContext(config);
            var model = EdmModelBuilder.Build(context);

            model.Dump();

            return model;
        }

        private static IEdmModel BuildValidModel(BoundedContextElementBuilder config)
        {
            var model = BuildModel(config);
            
            Assert.That(model, Is.Not.Null.And.Matches(Model.IsValid));

            return model;
        }

        public static class Entity
        {
            public static Func<IEdmEntityType, bool> ByName(string name)
            {
                return x => x.Name == name;
            }

//            public static Predicate<EdmEntityType> OfEntityType { get { return x => x.HasKey; } }
//
//            public static Predicate<EdmEntityType> OfComplexType { get { return x => !x.HasKey; } }

            public static class Property
            {
//                public static Func<EdmEntityPropertyInfo, bool> ByName(string name)
//                {
//                    return x => x.Name == name;
//                }
//
                public static Predicate<IEdmProperty> Members(params string[] names)
                {
                    return x => names.OrderBy(_ => _).SequenceEqual(((IEdmEnumType)x.Type.Definition).Members.Select(m => m.Name).OrderBy(_ => _));
                }
            }
        }

        #endregion
    }
}
