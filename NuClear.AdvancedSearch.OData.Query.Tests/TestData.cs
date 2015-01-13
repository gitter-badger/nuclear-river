using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.OData.Edm;

using Moq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

namespace NuClear.AdvancedSearch.OData.Query.Tests
{
    public sealed class Class1
    {
        public int Id { get; set; }
        public Class2 Class2 { get; set; }
    }

    public sealed class Class2
    {
        public int Id { get; set; }
    }

    public static class Repositories
    {
        public static IQueryable<Class1> Class1 = new[]
                                         {
                                             new Class1 { Id = 1 },
                                             new Class1 { Id = 2, Class2 = new Class2 { Id = 0 } },
                                             new Class1 { Id = 3 },
                                             new Class1 { Id = 4, Class2 = new Class2 { Id = 1 } },
                                         }.AsQueryable();

    }

    public static class Class1EdmModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = BoundedContextElement.Config
                .Name("Context")
                .ConceptualModel(
                    StructuralModelElement.Config.Elements(
                        EntityElement.Config
                            .Name("Class1")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int32).NotNull())
                            .Relation(EntityRelationElement.Config
                                .Name("Class2")
                                .DirectTo(
                                    EntityElement.Config
                                        .Name("Class2")
                                        .IdentifyBy("Id")
                                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int32).NotNull())
                                )
                                .AsOne())
                    )
                );

            var element = ProcessContext(builder);
            var model = EdmModelBuilder.Build(element);
            model.AddClrAnnotations(new[] { typeof(Class1), typeof(Class2) });

            return model;
        }

        # region copy/paste from EdmModelBuilderTests, refactor this later

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

        # endregion
    }
}
