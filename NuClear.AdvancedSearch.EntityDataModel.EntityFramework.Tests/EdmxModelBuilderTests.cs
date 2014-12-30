using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.EntityDataModel.EntityFramework.Building;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

using NUnit.Framework;

namespace EntityDataModel.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxModelBuilderTests
    {
        [Test]
        public void ShouldExposeEntities()
        {
            var config = NewContext("Library", NewEntity("Book"), NewEntity("Author"));

            var model = BuildEdmModel(config);

            Assert.NotNull(model);
            Assert.That(model.Container.EntitySets, Has.Count.EqualTo(2));
            Assert.That(model.Container.FindEntitySet("Book"), Is.Not.Null);
            Assert.That(model.Container.FindEntitySet("Author"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeEntityTypes()
        {
            var config = NewContext("Library", NewEntity("Book"), NewEntity("Author"));

            var model = BuildEdmModel(config);

            Assert.NotNull(model);
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Book"), Is.Not.Null.And.InstanceOf<EntityType>());
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Author"), Is.Not.Null.And.InstanceOf<EntityType>());
        }

        [Test]
        public void ShouldExposeEntityProperties()
        {
            var config = BoundedContextElement.Config
                .Name("Context")
                .Elements(
                    EntityElement.Config
                        .Name("Entity").IdentifyBy("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte).NotNull())
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                        );

            var model = BuildEdmModel(config);
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
            var config = NewContext("Context")
                .Elements(
                    NewEntity("Person")
                    .Property(EntityPropertyElement.Config
                        .Name("Gender")
                        .UsingEnum("GenderEnum")
                        .WithMember("Male", 1)
                        .WithMember("Female", 2)
                        )
                    );

            var model = BuildEdmModel(config);
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

            var model = BuildEdmModel(NewContext("Context", element));
            Assert.NotNull(model);

            var entity = model.FindDeclaredType("AdvancedSearch.Context.Entity");
            Assert.NotNull(entity);
            Assert.AreEqual(primitiveTypes.Length, entity.DeclaredProperties.Count());
        }

        #region Utils

        private static IMetadataSource MockSource(IMetadataElement context)
        {
            var source = new Mock<IMetadataSource>();
            source.Setup(x => x.Kind).Returns(new AdvancedSearchIdentity());
            source.Setup(x => x.Metadata).Returns(new Dictionary<Uri, IMetadataElement> { { IdBuilder.For<AdvancedSearchIdentity>(), context } });

            return source.Object;
        }

        private static IMetadataProvider CreateProvider(params IMetadataSource[] sources)
        {
            return new MetadataProvider(sources, new IMetadataProcessor[0]);
        }

        private static BoundedContextElement ProcessContext(BoundedContextElement context)
        {
            var provider = CreateProvider(MockSource(context));

            return provider.Metadata.Metadata.Values.OfType<BoundedContextElement>().FirstOrDefault();
        }

        private static DbModel BuildModel(BoundedContextElement context)
        {
            var model = EdmxModelBuilder.Build(ProcessContext(context));

            model.ConceptualModel.Dump();
            //model.Dump();

            return model;
        }

        private static EdmModel BuildEdmModel(BoundedContextElement context)
        {
            var model = EdmxModelBuilder.Build(ProcessContext(context));

            model.ConceptualModel.Dump();
            //model.Dump();

            return model.ConceptualModel;
        }

//        private static IEdmModel BuildValidModel(BoundedContextElementBuilder config)
//        {
//            var model = BuildModel(config);
//
//            Assert.That(model, Is.Not.Null.And.Matches(Model.IsValid));
//
//            return model;
//        }

        private static BoundedContextElementBuilder NewContext(string name, params EntityElementBuilder[] entities)
        {
            var config = BoundedContextElement.Config.Name(name);

            foreach (var entityElementBuilder in entities)
            {
                config.Elements(entityElementBuilder);
            }

            return config;
        }

        private static EntityElementBuilder NewEntity(string name, params EntityPropertyElementBuilder[] properties)
        {
            var config = EntityElement.Config.Name(name);

            if (properties.Length == 0)
            {
                config.Property(NewProperty("Id").NotNull()).IdentifyBy("Id");
            }

            foreach (var propertyElementBuilder in properties)
            {
                config.Property(propertyElementBuilder);
            }

            return config;
        }

        private static EntityPropertyElementBuilder NewProperty(string propertyName, EntityPropertyType propertyType = EntityPropertyType.Int64)
        {
            return EntityPropertyElement.Config.Name(propertyName).OfType(propertyType);
        }

        private static EntityRelationElementBuilder NewRelation(string relationName)
        {
            return EntityRelationElement.Config.Name(relationName);
        }

        public static class Property
        {
            public static Predicate<EdmProperty> OfType(PrimitiveTypeKind typeKind)
            {
                return x => x.PrimitiveType.PrimitiveTypeKind == typeKind;
            }

//            public static Predicate<IEdmProperty> OfKind(EdmPropertyKind propertyKind)
//            {
//                return x => x.PropertyKind == propertyKind;
//            }
//
//            public static Predicate<IEdmProperty> IsCollection()
//            {
//                return x => x.Type.Definition is IEdmCollectionType;
//            }

            public static Predicate<EdmProperty> Members(params string[] names)
            {
                return x => names.OrderBy(_ => _).SequenceEqual(x.EnumType.Members.Select(m => m.Name).OrderBy(_ => _));
            }
        }

        #endregion
    }
}
