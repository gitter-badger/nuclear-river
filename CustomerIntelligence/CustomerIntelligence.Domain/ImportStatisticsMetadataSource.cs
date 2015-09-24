using System;
using System.Collections.Generic;

using NuClear.CustomerIntelligence.Domain.DTO;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;
using NuClear.Replication.Metadata;
using NuClear.Replication.Metadata.Facts;

using Bit = NuClear.CustomerIntelligence.Domain.Model.Facts;
using Specs = NuClear.CustomerIntelligence.Domain.Specifications.Specs;

namespace NuClear.CustomerIntelligence.Domain
{
    public class ImportStatisticsMetadataSource : MetadataSourceBase<ImportStatisticsMetadataIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public ImportStatisticsMetadataSource()
        {
            HierarchyMetadata importStatisticsMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ImportStatisticsMetadataIdentity>())
                    .Childs(ImportStatisticsMetadata<Bit::FirmCategoryStatistics>
                                .Config
                                .HasSource<FirmStatisticsDto>(Specs.Map.Bit.FirmCategoryStatistics())
                                .Aggregated(Specs.Find.Bit.FirmCategoryStatistics.ByProject),

                            ImportStatisticsMetadata<Bit::ProjectCategoryStatistics>
                                .Config
                                .HasSource<CategoryStatisticsDto>(Specs.Map.Bit.ProjectCategoryStatistics())
                                .Aggregated(Specs.Find.Bit.ProjectCategoryStatistics.ByProject));

            _metadata = new Dictionary<Uri, IMetadataElement> { { importStatisticsMetadataRoot.Identity.Id, importStatisticsMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}