using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Moq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.EntityDataModel.EntityFramework.Building;

using NUnit.Framework;

namespace EntityDataModel.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxBuilderModelTests : EdmxBuilderBaseFixture
    {
        [Test]
        public void ShouldQueryData()
        {
            var typeProvider = new Mock<ITypeProvider>();
            typeProvider.Setup(x => x.Resolve(It.Is<EntityElement>(el => el.ResolveName() == "Firm"))).Returns(typeof(Firm));
            typeProvider.Setup(x => x.Resolve(It.Is<EntityElement>(el => el.ResolveName() == "Category"))).Returns(typeof(Category));

            BoundedContextElement config = NewContext("Library").ConceptualModel(NewModel(NewEntity("Firm")));

            var builder = CreateModelBuilder(typeProvider.Object);
            var model = builder.Build(ProcessContext(config));

            model.Dump();

            var connection = Effort.DbConnectionFactory.CreateTransient();
            using (var context = new DbContext(connection, model.Compile(), true))
            {
                var firms = context.Set<Category>().ToArray();
                Assert.That(firms, Is.Empty);
            }
        }

        private class Firm
        {
            public long Id { get; set; }
            public long OrganizationUnitId { get; set; }
            
            public ICollection<Category> Categories { get; set; }
        }

        private class Category
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}