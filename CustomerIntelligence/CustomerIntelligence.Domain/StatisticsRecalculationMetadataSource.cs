using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;

using Statistics = NuClear.CustomerIntelligence.Domain.Model.Statistics;
using Specs = NuClear.CustomerIntelligence.Domain.Specifications.Specs;

namespace NuClear.CustomerIntelligence.Domain
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
    public class StatisticsRecalculationMetadataSource : MetadataSourceBase<StatisticsRecalculationMetadataIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public StatisticsRecalculationMetadataSource()
        {
            HierarchyMetadata statisticsRecalculationMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<StatisticsRecalculationMetadataIdentity>())
                    .Childs(StatisticsRecalculationMetadata<Statistics::FirmCategoryStatistics>
                                .Config
                                .HasSource(Specs.Map.Facts.ToStatistics.FirmCategoryStatistics)
                                .HasTarget(Specs.Map.CI.ToStatistics.FirmCategoryStatistics)
                                .HasFilter(
                                    (projectId, categoryIds) =>
                                    categoryIds.Contains(null)
                                        ? Specs.Find.CI.FirmCategoryStatistics.ByProject(projectId)
                                        : Specs.Find.CI.FirmCategoryStatistics.ByProjectAndCategories(projectId, categoryIds)));

            _metadata = new Dictionary<Uri, IMetadataElement> { { statisticsRecalculationMetadataRoot.Identity.Id, statisticsRecalculationMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}