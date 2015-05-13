using Moq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.Tests.Data;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;
    using CI = CustomerIntelligence.Model;

    [TestFixture]
    internal partial class FactsTransformationTests
    {
        [Test]
        public void ShouldInitializeCategoryGroupIfCategoryGroupCreated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.CategoryGroups == Inquire(new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 1}));

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Initialize<CI::CategoryGroup>(1)));
        }

        [Test]
        public void ShouldDestroyCategoryGroupIfCategoryGroupDeleted()
        {
            var source = Mock.Of<IFactsContext>();

            FactsDb.Has(new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Destroy<CI::CategoryGroup>(1)));
        }

        [Test]
        public void ShouldRecalculateCategoryGroupIfCategoryGroupUpdated()
        {
            var source = Mock.Of<IFactsContext>(ctx => ctx.CategoryGroups == Inquire(new Facts::CategoryGroup { Id = 1, Name = "FooBar", Rate = 2 }));

            FactsDb.Has(new Facts::CategoryGroup { Id = 1, Name = "Name", Rate = 1 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::CategoryGroup>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::CategoryGroup>(1)));
        }
    }
}