using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.Engine.Tests;
using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Tests
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

            Assert.That(model, Is.Not.Null.And.Matches(EntityDataModel.OData.Tests.Model.IsValid));
        }

        #region Utils

        private static IEdmModel BuildModel(BoundedContextElement config)
        {
            var model = EdmModelBuilder.Build(config);

            model.Dump();

            return model;
        }

        #endregion
    }
}
