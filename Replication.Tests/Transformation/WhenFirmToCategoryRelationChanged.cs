using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using LinqToDB;

using Moq;

using Newtonsoft.Json;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Tests.Data;

using NUnit.Framework;

// ReSharper disable PossibleUnintendedReferenceComparison
namespace NuClear.AdvancedSearch.Replication.Tests.Transformation
{
    using Erm = CustomerIntelligence.Model.Erm;
    using Facts = CustomerIntelligence.Model.Facts;
    using CI = CustomerIntelligence.Model;

    [TestFixture]
    internal partial class WhenFirmToCategoryRelationChanged
    {
        private const string Context = @"{
            Projects: [ { Id: 1, OrganizationUnitId: 2 }, { Id: 3, OrganizationUnitId: 4 } ],
            Categories: [ { Id: 5 }, { Id: 6 } ],
            Firms: [ { Id: 7, OrganizationUnitId: 2 }, { Id: 8, OrganizationUnitId: 2 }, { Id: 9, OrganizationUnitId: 2 }, { Id: 10, OrganizationUnitId: 4 } ],
            FirmAddresses: [ { Id: 7, FirmId: 7 }, { Id: 8, FirmId: 8 }, { Id: 9, FirmId: 9 }, { Id: 10, FirmId: 10 } ],
            CategoryFirmAddresses: [
                { Id: 11, FirmAddressId: 7, CategoryId: 6 },
                { Id: 12, FirmAddressId: 8, CategoryId: 5 },
                { Id: 13, FirmAddressId: 8, CategoryId: 6 },
                { Id: 14, FirmAddressId: 9, CategoryId: 5 },
                { Id: 15, FirmAddressId: 10, CategoryId: 5 },
                { Id: 16, FirmAddressId: 10, CategoryId: 6 }
            ]
}";

        private static IEnumerable<TestCaseData> Cases()
        {
            yield return new TestCaseData(Context, Context, new UpdateFact(typeof(Facts::CategoryFirmAddress), 11));
            yield return new TestCaseData(Context, Context, new UpdateFact(typeof(Facts::FirmAddress), 7));
            yield return new TestCaseData(Context, Context, new UpdateFact(typeof(Facts::Firm), 7));
        }
            
        [TestCaseSource("Cases")]
        public void ItShouldAffectFirm(string sourceContext, string targetContext, FactOperation impact)
        {
            var transformation = CreateTransformation(sourceContext, targetContext); 

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.Member(new RecalculateAggregate(typeof(CI.Firm), 7)));
        }

        [TestCaseSource("Cases")]
        public void ItShouldAffectFirmsOfSameProjectAndSameCategory(string sourceContext, string targetContext, FactOperation impact)
        {
            var transformation = CreateTransformation(sourceContext, targetContext); 

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.Member(new RecalculateAggregate(typeof(CI.Firm), 8)));
        }

        [TestCaseSource("Cases")]
        public void ItShouldNotAffectFirmsOfOtherCategories(string sourceContext, string targetContext, FactOperation impact)
        {
            var transformation = CreateTransformation(sourceContext, targetContext);

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.No.Member(new RecalculateAggregate(typeof(CI.Firm), 9)));
        }

        [TestCaseSource("Cases")]
        public void ItShouldNotAffectFirmsOfOtherProjects(string sourceContext, string targetContext, FactOperation impact)
        {
            var transformation = CreateTransformation(sourceContext, targetContext);

            var aggregateOperations = transformation.Transform(new[] { impact }).ToList();

            Assert.That(aggregateOperations, Has.No.Member(new RecalculateAggregate(typeof(CI.Firm), 10)));
        }

        private static ErmFactsTransformation CreateTransformation(string sourceContext, string targetContext)
        {
            return new ErmFactsTransformation(sourceContext.ToErmFactsContext(), targetContext.ToErmFactsContext(), Mock.Of<IDataMapper>());
        }
    }
}
