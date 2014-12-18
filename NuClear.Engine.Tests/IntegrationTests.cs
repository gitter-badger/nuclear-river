using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Engine.Tests
{
    [TestFixture]
    internal class IntegrationTests
    {
        private BoundedContextElement _boundedContext;

        [SetUp]
        public void Setup()
        {
            var sources = new IMetadataSource[] { new AdvancedSearchMetadataSource() };
            var processors = new IMetadataProcessor[] { };

            var provider = new MetadataProvider(sources, processors);

            provider.TryGetMetadata(IdBuilder.For<AdvancedSearchIdentity>("CustomerIntelligence"), out _boundedContext);
        }

        [Test]
        public void ShouldBuildValidModelForEntity()
        {
            var model = BuildModel(_boundedContext);

            Assert.That(model, Is.Not.Null.And.Matches(Model.IsValid));
        }

        #region Utils

        private static IEdmModelSource Adapt(BoundedContextElement config)
        {
            return new AdvancedSearchMetadataSourceAdapter(config);
        }

        private static IEdmModel BuildModel(BoundedContextElement config)
        {
            var modelSource = Adapt(config);
            var modelBuilder = new EdmModelBuilder(modelSource);
            var model = modelBuilder.Build();

            model.Dump();

            return model;
        }

        #endregion
    }
}
