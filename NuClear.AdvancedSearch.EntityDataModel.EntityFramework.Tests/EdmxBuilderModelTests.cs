using System;
using System.Collections.Generic;
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
        private static readonly string DefaultDataUri = string.Format("res://{0}/Data", typeof(EdmxBuilderModelTests).Assembly.GetName().Name);

        [Test]
        public void ShouldQueryData()
        {
            var typeProvider = MockTypeProvider(typeof(Firm), typeof(Category));
            var config = (BoundedContextElement)NewContext("Library").ConceptualModel(NewModel(NewEntity("Firm")));

            var builder = CreateModelBuilder(typeProvider);
            var model = builder.Build(ProcessContext(config));

            model.Dump();

            using (var context = new DbContext(CreateDataConnection(), model.Compile(), true))
            {
                Assert.That(context.Set<Firm>().ToArray(), Has.Length.EqualTo(2));
            }
        }

        private static DbConnection CreateDataConnection()
        {
            return DbConnectionFactory.CreateTransient(new CsvDataLoader(DefaultDataUri));
        }

        private static ITypeProvider MockTypeProvider(params Type[] types)
        {
            var typeProvider = new Mock<ITypeProvider>();

            foreach (var type in types)
            {
                RegisterType(typeProvider, type);
            }

            return typeProvider.Object;
        }

        private static void RegisterType(Mock<ITypeProvider> typeProvider, Type type)
        {
            typeProvider.Setup(x => x.Resolve(It.Is<EntityElement>(el => el.ResolveName() == type.Name))).Returns(type);
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