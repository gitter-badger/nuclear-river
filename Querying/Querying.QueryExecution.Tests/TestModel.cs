using System;
using System.Collections.Generic;

using Microsoft.OData.Edm;

using Moq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Querying.Metadata;
using NuClear.Querying.OData.Building;

namespace NuClear.Querying.QueryExecution.Tests
{
    public static class TestModel
    {
        private static readonly Type[] ClrTypes = { typeof(MasterClass), typeof(NestedClass), typeof(EnumType) };

        public static IEdmModel EdmModel { get; private set; }

        static TestModel()
        {
            BoundedContextElement context = BoundedContextElement.Config
                .Name("Context")
                .ConceptualModel(
                    StructuralModelElement.Config
                    .Types((EnumTypeElement) EnumTypeElement.Config.Name("EnumType").Member("Member1", 0).Member("Member2", 1))
                    .Elements(
                        EntityElement.Config
                            .Name("MasterClass")
                            .HasKey("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int32))
                            .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                            .Property(EntityPropertyElement.Config.Name("EnumType").OfType<EnumTypeElement>(EnumTypeElement.Config.Name("EnumType")))
                            .Relation(EntityRelationElement.Config
                                .Name("NestedClass")
                                .DirectTo(
                                    EntityElement.Config
                                        .Name("NestedClass")
                                        .HasKey("Id")
                                        .Property(EntityPropertyElement.Config.Name("Id").OfType(ElementaryTypeKind.Int32))
                                        .Property(EntityPropertyElement.Config.Name("Name").OfType(ElementaryTypeKind.String))
                             )
                             .AsOne())
                    ));

            var provider = CreateProvider(MockSource(context));
            var contextId = context.Identity.Id;

            var edmModelBuilder = new EdmModelBuilder(provider);
            EdmModel = edmModelBuilder.Build(contextId).AnnotateByClrTypes(ClrTypes);
        }

        #region Metadata Utils

        private static IMetadataSource MockSource(IMetadataElement context)
        {
            var source = new Mock<IMetadataSource>();
            source.Setup(x => x.Kind).Returns(new AdvancedSearchIdentity());
            source.Setup(x => x.Metadata).Returns(new Dictionary<Uri, IMetadataElement> { { Metamodeling.Elements.Identities.Builder.Metadata.Id.For<AdvancedSearchIdentity>(), context } });

            return source.Object;
        }

        private static IMetadataProvider CreateProvider(params IMetadataSource[] sources)
        {
            return new MetadataProvider(sources, new IMetadataProcessor[0]);
        }

        #endregion
    }

    public sealed class MasterClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public EnumType EnumType { get; set; }

        public NestedClass NestedClass { get; set; }
    }

    public sealed class NestedClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum EnumType { Member1 , Member2 }
}