using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Identities;
using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Concrete.Hierarchy;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider.Sources;

using Specs = NuClear.CustomerIntelligence.Domain.Specifications.Specs;

namespace NuClear.CustomerIntelligence.Domain
{
    public class AggregateConstructionMetadataSource : MetadataSourceBase<ReplicationMetadataIdentity>
    {
        private readonly IReadOnlyDictionary<Uri, IMetadataElement> _metadata;

        public AggregateConstructionMetadataSource()
        {
            HierarchyMetadata aggregateConstructionMetadataRoot =
                HierarchyMetadata
                    .Config
                    .Id.Is(Metamodeling.Elements.Identities.Builder.Metadata.Id.For<ReplicationMetadataIdentity>("Aggregates"))
                    .Childs(AggregateMetadata<Firm>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Firms)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmActivities, Specs.Find.CI.FirmActivities)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmBalances, Specs.Find.CI.FirmBalances)
                                .HasValueObject(Specs.Map.Facts.ToCI.FirmCategories, Specs.Find.CI.FirmCategories),

                            AggregateMetadata<Client>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Clients)
                                .HasValueObject(Specs.Map.Facts.ToCI.ClientContacts, Specs.Find.CI.ClientContacts),

                            AggregateMetadata<Project>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Projects)
                                .HasValueObject(Specs.Map.Facts.ToCI.ProjectCategories, Specs.Find.CI.ProjectCategories),

                            AggregateMetadata<Territory>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.Territories),

                            AggregateMetadata<CategoryGroup>
                                .Config
                                .HasSource(Specs.Map.Facts.ToCI.CategoryGroups));

            _metadata = new Dictionary<Uri, IMetadataElement> { { aggregateConstructionMetadataRoot.Identity.Id, aggregateConstructionMetadataRoot } };
        }

        public override IReadOnlyDictionary<Uri, IMetadataElement> Metadata
        {
            get { return _metadata; }
        }
    }
}