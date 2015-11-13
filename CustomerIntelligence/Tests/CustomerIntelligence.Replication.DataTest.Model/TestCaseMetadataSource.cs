using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.DataTest.Metamodel.Dsl;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Provider.Sources;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    public sealed partial class TestCaseMetadataSource : MetadataSourceBase<TestCaseMetadataIdentity>
    {
        private static IEnumerable<IEnumerable<TestCaseMetadataElement>> Tests()
        {
            yield return With(MinimalFirmAggregate).Do(ErmToFact, FactToCi, BitToStatistics);
            yield return With(ProjectAggregate).Do(ErmToFact, FactToCi);
            yield return With(BornToFail).Do(ErmToFact);
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
            => Tests()
                .SelectMany(x => x)
                .ToDictionary(x => x.Identity.Id, x => (IMetadataElement)x);

        private static IEnumerable<ArrangeMetadataElement> With(params ArrangeMetadataElement[] elements)
        {
            return elements;
        }
    }

    internal static class Extensions
    {
        public static IEnumerable<TestCaseMetadataElement> Do(this IEnumerable<ArrangeMetadataElement> arranges, params ActMetadataElement[] acts)
        {
            foreach (var arrangeMetadataElement in arranges)
            {
                foreach (var actMetadataElement in acts)
                {
                    yield return new TestCaseMetadataElement(arrangeMetadataElement, actMetadataElement);
                }
            }
        }
    }
}
