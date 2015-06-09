using System.Linq;

using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;

using NUnit.Framework;


// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation.CustomCase
{
    using Facts = CustomerIntelligence.Model.Facts;

    [TestFixture]
    internal partial class FactsTransformationTests
    {
        [Test]
        public void ShouldInitializeClientIfClientCreated()
        {
            var source = new Mock<IErmFactsContext>();
            source.SetupGet(x => x.Firms).Returns(new[] { new Facts.Firm { Id = 1 } }.AsQueryable());
            var target = new Mock<IErmFactsContext>();
            target.SetupGet(x => x.Firms).Returns(new[] { new Facts.Firm { Id = 1 } }.AsQueryable());
            var dataMapper = new Mock<IDataMapper>();

            var transformation = new ErmFactsTransformation(source.Object, target.Object, dataMapper.Object);

            transformation.Transform(new[] { new FactOperation(typeof(Facts.Firm), 1), });

            dataMapper.Verify(x => x.Update(new Facts.Firm { Id = 1 }), Times.AtLeastOnce());
            dataMapper.Verify(x => x.Insert(It.IsAny<Facts.Firm>()), Times.Never());
        }
    }
}