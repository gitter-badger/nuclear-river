using System.Data.Entity.Core.Metadata.Edm;

using NUnit.Framework;

namespace EntityDataModel.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxBuilderStoreModelTests : EdmxBuilderBaseFixture
    {
        [Test]
        public void ShouldExposeEntities()
        {
            var config = NewContext("Library").StoreModel(NewModel(NewEntity("Book"), NewEntity("Author")));

            var model = BuildStoreModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));
            Assert.That(model.Container.EntitySets, Has.Count.EqualTo(2));
            Assert.That(model.FindEntitySet("Book"), Is.Not.Null);
            Assert.That(model.FindEntitySet("Author"), Is.Not.Null);
        }

        [Test]
        public void ShouldExposeEntityTypes()
        {
            var config = NewContext("Library").StoreModel(NewModel(NewEntity("Book"), NewEntity("Author")));

            var model = BuildStoreModel(config);

            Assert.That(model, Is.Not.Null.And.Matches(StoreModel.IsValid));
            Assert.That(model.EntityTypes, Has.Count.EqualTo(2));
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Store.Book"), Is.Not.Null.And.InstanceOf<EntityType>());
            Assert.That(model.FindDeclaredType("AdvancedSearch.Library.Store.Author"), Is.Not.Null.And.InstanceOf<EntityType>());
        }
    }
}