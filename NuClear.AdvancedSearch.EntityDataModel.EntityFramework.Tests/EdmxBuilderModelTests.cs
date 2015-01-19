using System.Data.Common;
using System.Data.Entity;
using System.Linq;

using Effort;
using Effort.DataLoaders;

using Moq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.EntityDataModel.EntityFramework.Building;

using NUnit.Framework;

namespace EntityDataModel.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxBuilderModelTests : EdmxBuilderBaseFixture
    {
        private const string DefaultDataUri = "res://EntityDataModel.EntityFramework.Tests/Data";

        private static DbConnection CreateDataConnection()
        {
            return DbConnectionFactory.CreateTransient(new CsvDataLoader(DefaultDataUri));
        }

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

            using (var context = new DbContext(CreateDataConnection(), model.Compile(), true))
            {
                Assert.That(context.Set<Firm>().ToArray(), Has.Length.EqualTo(2));
            }
        }

        private class Firm
        {
            public long Id { get; set; }
            public long OrganizationUnitId { get; set; }
            
            //public ICollection<Category> Categories { get; set; }
        }

        private class Category
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}