using NUnit.Framework;

namespace EntityDataModel.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxBuilderMappingTests : EdmxBuilderBaseFixture
    {
        [Test]
        public void ShouldExposeEntitySets()
        {
            const string Book = "Book";
            const string Name = "Name";

            var config = NewContext("Library")
                .ConceptualModel(NewModel(NewEntity("Book")))
                .StoreModel(NewModel(NewEntity("Book")))
                .Map("Book", "Book");

            var model = BuildModel(config);

            Assert.That(model, Is.Not.Null);
        }
    }
}