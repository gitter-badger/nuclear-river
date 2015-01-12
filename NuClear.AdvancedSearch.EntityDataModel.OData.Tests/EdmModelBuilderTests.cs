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
            var model = BuildValidModel(BoundedContextElement.Config.Name("ContextName").ConceptualModel(StructuralModelElement.Config));
            
            Assert.NotNull(model);
            Assert.That(model.DeclaredNamespaces.SingleOrDefault(), Is.Not.Null.And.EqualTo("AdvancedSearch.ContextName"));
            Assert.That(model.EntityContainer, Is.Not.Null);
            Assert.That(model.EntityContainer.Elements, Has.Count.EqualTo(0));
        }

        [Test, ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "entity name was not specified")]
        public void ShouldFailIfNoEntityName()
        {
            BuildModel(NewContext("Context", EntityElement.Config));
        }

        [Test]
        public void ShouldExposeEntities()
        {
            var config = NewContext("Library", NewEntity("Book"), NewEntity("Author"));

            var model = BuildValidModel(config);
            
            Assert.NotNull(model);
            Assert.That(model.EntityContainer.Elements, Has.Count.EqualTo(2));
            Assert.That(model.EntityContainer.FindEntitySet("Book"), Is.Not.Null);
            Assert.That(model.EntityContainer.FindEntitySet("Author"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeEntityTypes()
        {
            var config = NewContext("Library", NewEntity("Book"), NewEntity("Author"));

            var model = BuildValidModel(config);
            
            Assert.NotNull(model);
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Book"), Is.Not.Null.And.InstanceOf<IEdmEntityType>());
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Author"), Is.Not.Null.And.InstanceOf<IEdmEntityType>());
        }

        [Test, ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "property name was not specified")]
        public void ShouldFailIfNoPropertyName()
        {
            BuildModel(NewContext("Context", 
                EntityElement.Config
                    .Name("Entity")
                    .Property(EntityPropertyElement.Config)
                ));
        }

        [Test]
        public void ShouldExposeEntityProperties()
        {
            var config = NewContext("Context",
                    EntityElement.Config
                        .Name("Entity").IdentifyBy("Id")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Byte).NotNull())
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                        );

            var model = BuildValidModel(config);
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
            var config = NewContext("Context",
                    NewEntity("Person")
                    .Property(EntityPropertyElement.Config
                        .Name("Gender")
                        .UsingEnum("GenderEnum")
                        .WithMember("Male", 1)
                        .WithMember("Female", 2)
                        )
                    );

            var model = BuildValidModel(config);
            Assert.NotNull(model);

            var entity = model.FindDeclaredType("AdvancedSearch.Context.Person") as IEdmEntityType;
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

            var model = BuildValidModel(NewContext("Context", element));
            Assert.NotNull(model);

            var entity = model.FindDeclaredType("AdvancedSearch.Context.Entity") as IEdmEntityType;
            Assert.NotNull(entity);
            Assert.AreEqual(primitiveTypes.Length, entity.DeclaredProperties.Count());
        }

        [Test]
        public void ShouldExposeRelatedTargetType()
        {
            var config =
                NewContext("Context", 
                    NewEntity("Entity")
                    .Relation(NewRelation("ToValueAsOne").DirectTo(NewEntity("RelatedValue", NewProperty("Name", EntityPropertyType.String))).AsOneOptionally())
                    .Relation(NewRelation("ToValueAsMany").DirectTo(NewEntity("RelatedValue", NewProperty("Name", EntityPropertyType.String))).AsMany())
                    .Relation(NewRelation("ToEntityAsOne").DirectTo(NewEntity("RelatedEntity")).AsOneOptionally())
                    .Relation(NewRelation("ToEntityAsMany").DirectTo(NewEntity("RelatedEntity")).AsMany())
                    );

            var model = BuildValidModel(config);

            Assert.That(model.FindDeclaredType("AdvancedSearch.Context.RelatedValue"), Is.Not.Null.And.InstanceOf<IEdmComplexType>());
            Assert.That(model.FindDeclaredType("AdvancedSearch.Context.RelatedEntity"), Is.Not.Null.And.InstanceOf<IEdmEntityType>());
            Assert.That(model.FindDeclaredType("AdvancedSearch.Context.Entity"), Is.Not.Null.And.InstanceOf<IEdmEntityType>());
            
            var entityType = (IEdmStructuredType)model.FindDeclaredType("AdvancedSearch.Context.Entity");
            Assert.That(entityType.FindProperty("ToValueAsOne"), Is.Not.Null
                .And.Matches(Property.OfKind(EdmPropertyKind.Structural))
                .And.Not.Matches(Property.IsCollection()));
            Assert.That(entityType.FindProperty("ToValueAsMany"), Is.Not.Null
                .And.Matches(Property.OfKind(EdmPropertyKind.Structural))
                .And.Matches(Property.IsCollection()));
            Assert.That(entityType.FindProperty("ToEntityAsOne"), Is.Not.Null
                .And.Matches(Property.OfKind(EdmPropertyKind.Navigation))
                .And.Not.Matches(Property.IsCollection()));
            Assert.That(entityType.FindProperty("ToEntityAsMany"), Is.Not.Null
                .And.Matches(Property.OfKind(EdmPropertyKind.Navigation))
                .And.Matches(Property.IsCollection()));
        }

        [Test]
        public void ShouldBuildValidModelForCustomerIntelligenceContext()
        {
            var provider = CreateProvider(new AdvancedSearchMetadataSource());

            BoundedContextElement boundedContext;
            provider.TryGetMetadata(IdBuilder.For<AdvancedSearchIdentity>("CustomerIntelligence"), out boundedContext);
            
            var context = ProcessContext(boundedContext);
            var model = BuildModel(context);

            Assert.That(model, Is.Not.Null.And.Matches(Model.IsValid));
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

        private static IEdmModel BuildModel(BoundedContextElement config)
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

        private static BoundedContextElementBuilder NewContext(string name, params EntityElementBuilder[] entities)
        {
            var model = StructuralModelElement.Config;
            
            foreach (var entityElementBuilder in entities)
            {
                model.Elements(entityElementBuilder);
            }

            var config = BoundedContextElement.Config.Name(name);
            config.ConceptualModel(model);
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
            public static Predicate<IEdmProperty> OfKind(EdmPropertyKind propertyKind)
            {
                return x => x.PropertyKind == propertyKind;
            }

            public static Predicate<IEdmProperty> IsCollection()
            {
                return x => x.Type.Definition is IEdmCollectionType;
            }

            public static Predicate<IEdmProperty> Members(params string[] names)
            {
                return x => names.OrderBy(_ => _).SequenceEqual(((IEdmEnumType)x.Type.Definition).Members.Select(m => m.Name).OrderBy(_ => _));
            }
        }

        #endregion
    }
}
