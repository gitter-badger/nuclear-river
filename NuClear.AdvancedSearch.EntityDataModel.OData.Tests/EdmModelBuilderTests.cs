using NuClear.AdvancedSearch.EntityDataModel.OData.Building;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.EntityDataModel.OData.Tests
{
    [TestFixture]
    internal class EdmModelBuilderTests
    {
        private EdmModelBuilder _modelBuilder;

        [SetUp]
        public void Setup()
        {
            _modelBuilder = new EdmModelBuilder();
        }

        [Test]
        public void ShouldBuildModel()
        {
            var model = _modelBuilder.Build();
            Assert.IsNotNull(model);
        }
    }
}
