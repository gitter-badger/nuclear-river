using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Linq;

using Effort;

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

        static TestModel()
        {
            var builder = BoundedContextElement.Config
                .Name("Context")
                .ConceptualModel(
                    StructuralModelElement.Config.Elements(
                        EntityElement.Config
                            .Name("TestClass1")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int32).NotNull())
                            // TODO: актализировать когда будет поддержка relations
                            //.Relation(EntityRelationElement.Config
                            //    .Name("TestClass2")
                            //    .DirectTo(
                            //        EntityElement.Config
                            //            .Name("TestClass2")
                            //            .IdentifyBy("Id")
                            //            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int32).NotNull())
                            //    )
                            //    .AsOne())
                    )
                )
                .StoreModel(
                    StructuralModelElement.Config.Elements(
                         EntityElement.Config
                            .Name("dbo.TestClass1")
                            .IdentifyBy("Id")
                            .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int32).NotNull())
                    )
                );

            var element = ProcessContext(builder);
            var dbModel = BuildDbModel(element);

            var clrTypes = dbModel.GetClrTypes().ToArray();
            EdmModel = EdmModelBuilder.Build(element);
            EdmModel.AddClrAnnotations(clrTypes);

            EFModel = dbModel.Compile();
        }

        private static DbModel BuildDbModel(BoundedContextElement element)
        {
            using (var connection = DbConnectionFactory.CreateTransient())
            {
                var providerManifestToken = DbConfiguration.DependencyResolver.GetService<IManifestTokenResolver>().ResolveManifestToken(connection);
                var providerInvariantName = DbConfiguration.DependencyResolver.GetService<IProviderInvariantName>(DbProviderServices.GetProviderFactory(connection)).Name;
                var dbProviderInfo = new DbProviderInfo(providerInvariantName, providerManifestToken);

                return EdmxModelBuilder.Build(element, dbProviderInfo);
            }
        }

        # region copy/paste from EdmModelBuilderTests, refactor this later

        private static BoundedContextElement ProcessContext(BoundedContextElement context)
        {
            var provider = CreateProvider(MockSource(context));

            return Enumerable.OfType<BoundedContextElement>(provider.Metadata.Metadata.Values).FirstOrDefault();
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
