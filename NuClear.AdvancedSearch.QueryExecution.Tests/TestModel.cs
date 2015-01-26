using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;

using Effort.Provider;

using Microsoft.OData.Edm;

using Moq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.EntityDataModel.EntityFramework.Building;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.AdvancedSearch.QueryExecution.Tests
{
    public static class TestModel
    {
        public static readonly DbCompiledModel EFModel;
        public static readonly IEdmModel EdmModel;

        private static readonly Type[] ClrTypes = { typeof(TestClass1), typeof(TestClass2), typeof(Enum1) };

        static TestModel()
        {
            var builder = BoundedContextElement.Config
                .Name("Context")
                .ConceptualModel(
                    StructuralModelElement.Config.Elements(
                        EntityElement.Config
                            .Name("TestClass1")
                            .HasKey("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int32))
                            .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                            .Property(EntityPropertyElement.Config.Name("Enum1").UsingEnum("Enum1").WithMember("Member1", 0).WithMember("Member2", 1))
                            .Relation(EntityRelationElement.Config
                                .Name("TestClass2")
                                .DirectTo(
                                    EntityElement.Config
                                        .Name("TestClass2")
                                        .HasKey("Id")
                                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int32))
                                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                             )
                             .AsOne())
                    ))

                .StoreModel(
                    StructuralModelElement.Config.Elements(
                         EntityElement.Config
                            .Name("dbo.TestClass1")
                            .HasKey("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int32))
                            .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                            .Property(EntityPropertyElement.Config.Name("Enum1").OfType(EntityPropertyType.Int32)),
                         EntityElement.Config
                            .Name("dbo.TestClass2")
                            .HasKey("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int32))
                            .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                    )
                );

            var element = ProcessContext(builder);
            var provider = CreateProvider(MockSource(element));
            var contextId = provider.Metadata.Metadata.Values.OfType<BoundedContextElement>().Single().Identity.Id;

            var dbProviderInfo = GeEffortProviderInfo();
            var typeProvider = GetTypeProvider(ClrTypes);
            var edmxModelBuilder = new EdmxModelBuilder(dbProviderInfo, provider, typeProvider);
            var dbModel = edmxModelBuilder.Build(contextId);
            var clrTypes = dbModel.GetClrTypes();

            var edmModelBuilder = new EdmModelBuilder(provider);
            EdmModel = edmModelBuilder.Build(contextId);
            EdmModel.AddClrAnnotations(clrTypes);

            EFModel = dbModel.Compile();
        }

        private static DbProviderInfo GeEffortProviderInfo()
        {
            EffortProviderConfiguration.RegisterProvider();
            return new DbProviderInfo(EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
        }

        private static ITypeProvider GetTypeProvider(IEnumerable<Type> types)
        {
            var mock = new Mock<ITypeProvider>();

            foreach (var type in types)
            {
                mock.Setup(x => x.Resolve(It.Is<EntityElement>(y => y.ResolveName() == type.Name))).Returns(type);
            }

            return mock.Object;
        }

        #region copy/paste from EdmModelBuilderTests, refactor this later

        private static BoundedContextElement ProcessContext(BoundedContextElement context)
        {
            var provider = CreateProvider(MockSource(context));

            return provider.Metadata.Metadata.Values.OfType<BoundedContextElement>().FirstOrDefault();
        }

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

        private static Uri LookupContextId(IMetadataProvider provider)
        {
            return provider.Metadata.Metadata.Values.OfType<BoundedContextElement>().Single().Identity.Id;
        }

        #endregion
    }

    public enum Enum1 { Member1 , Member2 }

    public sealed class TestClass1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Enum1 Enum1 { get; set; }

        public TestClass2 TestClass2 { get; set; }
    }

    public sealed class TestClass2
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}