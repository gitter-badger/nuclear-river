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
        public void ShouldInitializeCategoryIfCategoryCreated()
        {
            var source = Mock.Of<IErmFactsContext>(ctx => ctx.Categories == Inquire(new Facts::Category { Id = 1 })
                && ctx.Projects == Inquire(new Facts::Project { Id = 2, OrganizationUnitId = 3 })
                && ctx.CategoryOrganizationUnits == Inquire(new Facts::CategoryOrganizationUnit{ CategoryId = 1, OrganizationUnitId = 3, Id = 4}));

            FactsDb
                .Has(new Facts::Project { Id = 2, OrganizationUnitId = 3 })
                .Has(new Facts::CategoryOrganizationUnit { CategoryId = 1, OrganizationUnitId = 3, Id = 4 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Create<Facts::Category>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Project>(2)));
        }

        [Test]
        public void ShouldRecalculateCategoryIfCategoryUpdated()
        {
            var source = Mock.Of<IErmFactsContext>(ctx => ctx.Categories == Inquire(new Facts::Category { Id = 1 })
                && ctx.Projects == Inquire(new Facts::Project { Id = 2, OrganizationUnitId = 3 })
                && ctx.CategoryOrganizationUnits == Inquire(new Facts::CategoryOrganizationUnit { CategoryId = 1, OrganizationUnitId = 3, Id = 4 }));

            FactsDb
                .Has(new Facts::Category { Id = 1 })
                .Has(new Facts::Project { Id = 2, OrganizationUnitId = 3 })
                .Has(new Facts::CategoryOrganizationUnit { CategoryId = 1, OrganizationUnitId = 3, Id = 4 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Update<Facts::Category>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Project>(2), Aggregate.Recalculate<CI::Project>(2)));
        }

        [Test]
        public void ShouldDestroyCategoryIfCategoryDeleted()
        {
            var source = Mock.Of<IErmFactsContext>(ctx => 
                ctx.Projects == Inquire(new Facts::Project { Id = 2, OrganizationUnitId = 3 })
                && ctx.CategoryOrganizationUnits == Inquire(new Facts::CategoryOrganizationUnit { CategoryId = 1, OrganizationUnitId = 3, Id = 4 }));

            FactsDb
                .Has(new Facts::Category { Id = 1 })
                .Has(new Facts::Project { Id = 2, OrganizationUnitId = 3 })
                .Has(new Facts::CategoryOrganizationUnit { CategoryId = 1, OrganizationUnitId = 3, Id = 4 });

            Transformation.Create(source, FactsDb)
                          .Transform(Fact.Delete<Facts::Category>(1))
                          .Verify(Inquire(Aggregate.Recalculate<CI::Project>(2)));
        }
    }
}