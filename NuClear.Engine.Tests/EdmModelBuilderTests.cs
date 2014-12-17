using Microsoft.OData.Edm;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.AdvancedSearch.EntityDataModel.OData.Building;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.Engine.Tests
{
    [TestFixture]
    internal class EdmModelBuilderTests
    {
        [Test]
        public void ShouldBuildValidModelForEntity()
        {
            var config = BoundedContextElement.Config
                .Name("Context")
                .Elements(
                    EntityElement.Config
                        .Name("Entity")
                        .Property(EntityPropertyElement.Config.Name("Id").OfType(EntityPropertyType.Int64).NotNull())
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String))
                        .IdentifyBy("Id"));
            var model = BuildModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(Model.IsValid));
        }

        [Test]
        public void ShouldBuildValidModelForComplexType()
        {
            var config = BoundedContextElement.Config
                .Name("Context")
                .Elements(
                    EntityElement.Config
                        .Name("ValueObject")
                        .Property(EntityPropertyElement.Config.Name("Name").OfType(EntityPropertyType.String)));
            var model = BuildModel(config);
            
            Assert.That(model, Is.Not.Null.And.Matches(Model.IsValid));
        }

        #region Utils

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

        #endregion
    }
}
