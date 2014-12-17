using System.Diagnostics;

using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Engine.Tests
{
    [TestFixture]
    internal class EngineTests
    {
        private EdmModelSourceFactory _factory;

        [SetUp]
        public void Setup()
        {
            var sources = new[] { new AdvancedSearchMetadataSource() };
            var processors = new IMetadataProcessor[] { };

            var provider = new MetadataProvider(sources, processors);
            _factory = new EdmModelSourceFactory(provider);
        }

        [Test]
        public void ShouldBeValid()
        {
            var modelSource = _factory.Create("CustomerIntelligence");
            var modelBuilder = new EdmModelBuilder(modelSource);

            var model = modelBuilder.Build();
            Debug.WriteLine(model.Dump());

            Assert.NotNull(model);
            Assert.That(model, Model.IsValid);
        }

        [Test]
        public void Should()
        {
            var config = BoundedContextElement.Config
                .Name("CustomerIntelligence")
                .Elements(
                    EntityElement.Config
                        .Name("Firm")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                        .Property(EntityPropertyElement.Config.Name("OrganizationUnitId").OfType(EntityPropertyType.Int64))
                        .Property(EntityPropertyElement.Config.Name("TerritoryId").OfType(EntityPropertyType.Int64))
                        .Property(EntityPropertyElement.Config.Name("CreatedOn").OfType(EntityPropertyType.DateTime))
                        .Property(EntityPropertyElement.Config.Name("LastQualifiedOn").OfType(EntityPropertyType.DateTime))
                        .Property(EntityPropertyElement.Config.Name("LastDistributedOn").OfType(EntityPropertyType.DateTime))
                        .Property(EntityPropertyElement.Config.Name("HasWebsite").OfType(EntityPropertyType.Boolean))
                        .Property(EntityPropertyElement.Config.Name("HasPhone").OfType(EntityPropertyType.Boolean))
                        .Property(EntityPropertyElement.Config.Name("CategoryGroup").OfType(EntityPropertyType.Byte))
                        .Property(EntityPropertyElement.Config.Name("AddressCount").OfType(EntityPropertyType.Int32))
                        .IdentifyBy("Id"));
            var model = BuildModel(config);
            
            Assert.NotNull(model);
            Assert.That(model, Model.IsValid);
        }

        private static IEdmModelSource Adapt(BoundedContextElementBuilder config)
        {
            return new AdvancedSearchMetadataSourceAdapter(config);
        }

        private static IEdmModel BuildModel(BoundedContextElementBuilder config)
        {
            var modelSource = Adapt(config);
            var modelBuilder = new EdmModelBuilder(modelSource);
            var model = modelBuilder.Build();
            
            model.Dump();

            return model;
        }
    }
}
