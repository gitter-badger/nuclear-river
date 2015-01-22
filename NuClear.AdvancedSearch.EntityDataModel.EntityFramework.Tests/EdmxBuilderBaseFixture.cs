using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Effort;
using Effort.DataLoaders;
using Effort.Provider;

using EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence;

using Moq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.EntityDataModel.EntityFramework.Building;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace EntityDataModel.EntityFramework.Tests
{
    public class EdmxBuilderBaseFixture
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            EffortProviderConfiguration.RegisterProvider();
        }

        protected static DbProviderFactory EffortFactory
        {
            get
            {
                return DbProviderFactories.GetFactory(EffortProviderConfiguration.ProviderInvariantName);
            }
        }

        protected static DbProviderInfo EffortProvider
        {
            get
            {
                return new DbProviderInfo(EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
            }
        }

        protected static IMetadataSource CustomerIntelligenceMetadataSource
        {
            get
            {
                return new AdvancedSearchMetadataSource();
            }
        }

        protected static ITypeProvider CustomerIntelligenceTypeProvider
        {
            get
            {
                return MockTypeProvider(
                    typeof(Account),
                    typeof(Category),
                    typeof(Client),
                    typeof(Contact),
                    typeof(Firm));
            }
        }

        private static ITypeProvider MockTypeProvider(params Type[] types)
        {
            var typeProvider = new Mock<ITypeProvider>();

            foreach (var type in types)
            {
                RegisterType(typeProvider, type);
            }

            return typeProvider.Object;
        }

        private static void RegisterType(Mock<ITypeProvider> typeProvider, Type type)
        {
            typeProvider.Setup(x => x.Resolve(It.Is<EntityElement>(el => el.ResolveName() == type.Name))).Returns(type);
        }

        protected static DbModel BuildModel(IMetadataSource source, ITypeProvider typeProvider = null)
        {
            var metadataProvider = CreateMetadataProvider(source);
            var context = LookupContext(metadataProvider);

            var builder = CreateBuilder(typeProvider);
            var model = builder.Build(metadataProvider, context.Identity.Id);

            model.Dump();

            return model;
        }

        protected static DbModel BuildModel(BoundedContextElement context, ITypeProvider typeProvider = null)
        {
            var model = CreateModel(context, typeProvider);

            model.Dump();

            return model;
        }

        protected static EdmModel BuildConceptualModel(BoundedContextElement context)
        {
            var model = CreateModel(context);

            model.ConceptualModel.Dump(EdmxExtensions.EdmModelType.Conceptual);

            return model.ConceptualModel;
        }

        protected static EdmModel BuildStoreModel(BoundedContextElement context)
        {
            var model = CreateModel(context);

            model.StoreModel.Dump(EdmxExtensions.EdmModelType.Store);

            return model.StoreModel;
        }

        protected static class ConceptualModel
        {
            public static Constraint IsValid { get { return new ModelValidationConstraint(); } }

            private class ModelValidationConstraint : Constraint
            {
                private const int MaxErrorsToDisplay = 5;
                private IReadOnlyCollection<string> _errors;

                public override bool Matches(object value)
                {
                    var model = value as EdmModel;
                    if (model == null)
                    {
                        throw new ArgumentException("The specified actual value is not a model.", "value");
                    }

                    return model.IsValidCsdl(out _errors);
                }

                public override void WriteDescriptionTo(MessageWriter writer)
                {
                    writer.Write("valid");
                }

                public override void WriteActualValueTo(MessageWriter writer)
                {
                    if (_errors.Count == 0)
                    {
                        return;
                    }

                    writer.WriteLine("The model containing errors:");
                    foreach (var error in _errors.Take(MaxErrorsToDisplay))
                    {
                        writer.WriteMessageLine(2, error);
                    }
                }
            }
        }

        protected static class StoreModel
        {
            public static Constraint IsValid { get { return new ModelValidationConstraint(); } }

            private class ModelValidationConstraint : Constraint
            {
                private const int MaxErrorsToDisplay = 5;
                private IReadOnlyCollection<string> _errors;

                public override bool Matches(object value)
                {
                    var model = value as EdmModel;
                    if (model == null)
                    {
                        throw new ArgumentException("The specified actual value is not a model.", "value");
                    }

                    return model.IsValidSsdl(out _errors);
                }

                public override void WriteDescriptionTo(MessageWriter writer)
                {
                    writer.Write("valid");
                }

                public override void WriteActualValueTo(MessageWriter writer)
                {
                    if (_errors.Count == 0)
                    {
                        return;
                    }
                    
                    writer.WriteLine("The model containing errors:");
                    foreach (var error in _errors.Take(MaxErrorsToDisplay))
                    {
                        writer.WriteMessageLine(2, error);
                    }
                }
            }
        }

        protected static class Property
        {
            public static Predicate<EdmProperty> OfType(PrimitiveTypeKind typeKind)
            {
                return x => x.PrimitiveType.PrimitiveTypeKind == typeKind;
            }

            public static Predicate<EdmProperty> IsKey()
            {
                return x => (x.DeclaringType as EntityType) != null && ((EntityType)x.DeclaringType).KeyMembers.Contains(x);
            }

            public static Predicate<EdmProperty> Members(params string[] names)
            {
                return x => names.OrderBy(_ => _).SequenceEqual(x.EnumType.Members.Select(m => m.Name).OrderBy(_ => _));
            }
        }

        #region Test Data

        private static readonly string DefaultTestDataUri = string.Format("res://{0}/Data", typeof(EdmxBuilderBaseFixture).Assembly.GetName().Name);

        protected static DbConnection CreateMemoryConnection()
        {
            return DbConnectionFactory.CreateTransient();
        }

        protected static DbConnection CreateConnection(string path = null)
        {
            var dataLoader = new CsvDataLoader(path ?? DefaultTestDataUri);
            var cachingLoader = new CachingDataLoader(dataLoader);
            return DbConnectionFactory.CreateTransient(cachingLoader);
        }

        #endregion

        #region Metadata Helpers

        protected static BoundedContextElementBuilder NewContext(string name)
        {
            return BoundedContextElement.Config.Name(name);
        }

        protected static StructuralModelElementBuilder NewModel(params EntityElementBuilder[] entities)
        {
            var model = StructuralModelElement.Config;

            foreach (var entityElementBuilder in entities)
            {
                model.Elements(entityElementBuilder);
            }

            return model;
        }

        protected static EntityElementBuilder NewEntity(string name, params EntityPropertyElementBuilder[] properties)
        {
            var config = EntityElement.Config.Name(name);

            if (properties.Length == 0)
            {
                config.Property(NewProperty("Id")).HasKey("Id");
            }

            foreach (var propertyElementBuilder in properties)
            {
                config.Property(propertyElementBuilder);
            }

            return config;
        }

        protected static EntityPropertyElementBuilder NewProperty(string propertyName, EntityPropertyType propertyType = EntityPropertyType.Int64)
        {
            return EntityPropertyElement.Config.Name(propertyName).OfType(propertyType);
        }

        protected static EntityRelationElementBuilder NewRelation(string relationName)
        {
            return EntityRelationElement.Config.Name(relationName);
        }

        #endregion

        #region Utils

        private static EdmxModelBuilder CreateBuilder(ITypeProvider typeProvider = null)
        {
            return new EdmxModelBuilder(EffortProvider, typeProvider);
        }

        private static DbModel CreateModel(IMetadataElement context, ITypeProvider typeProvider = null)
        {
            var metadataSource = MockSource(context);
            var metadataProvider = CreateMetadataProvider(metadataSource);
            var boundedContext = LookupContext(metadataProvider);

            var builder = CreateBuilder(typeProvider);
            var model = builder.Build(metadataProvider, boundedContext.Identity.Id);

            return model;
        }

        protected static BoundedContextElement LookupContext(IMetadataProvider provider)
        {
            return provider.Metadata.Metadata.Values.OfType<BoundedContextElement>().Single();
        }

        protected static IMetadataProvider CreateMetadataProvider(params IMetadataSource[] sources)
        {
            return new MetadataProvider(sources, new IMetadataProcessor[0]);
        }

        private static IMetadataSource MockSource(IMetadataElement context)
        {
            var source = new Mock<IMetadataSource>();
            source.Setup(x => x.Kind).Returns(new AdvancedSearchIdentity());
            source.Setup(x => x.Metadata).Returns(new Dictionary<Uri, IMetadataElement> { { IdBuilder.For<AdvancedSearchIdentity>(), context } });

            return source.Object;
        }

        #endregion
    }
}