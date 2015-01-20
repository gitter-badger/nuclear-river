using System.Data.Entity;
using System.Linq;

using EntityDataModel.EntityFramework.Tests.Model.CustomerIntelligence;

using NUnit.Framework;

namespace EntityDataModel.EntityFramework.Tests
{
    [TestFixture]
    internal class EdmxBuilderModelTests : EdmxBuilderBaseFixture
    {
        [Test]
        public void ShouldQueryData()
        {
            var model = BuildModel(CustomerIntelligenceMetadataSource, CustomerIntelligenceTypeProvider);

            using (var connection = CreateConnection())
            using (var context = new DbContext(connection, model.Compile(), false))
            {
                var firms = context.Set<Firm>().ToArray();

                Assert.That(firms, Has.Length.EqualTo(2));
            }
        }
    }
}