using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    public static partial class Specs
    {
        public static partial class Find
        {
            public static partial class CI
            {
                public static FindSpecification<CategoryGroup> CategoryGroups(IReadOnlyCollection<long> ids)
                {
                    return API.Specifications.Specs.Find.ByIds<CategoryGroup>(ids);
                }

                public static FindSpecification<Client> Clients(IReadOnlyCollection<long> ids)
                {
                    return API.Specifications.Specs.Find.ByIds<Client>(ids);
                }

                public static FindSpecification<ClientContact> ClientContacts(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<ClientContact>(x => aggregateIds.Contains(x.ClientId));
                }

                public static FindSpecification<Firm> Firms(IReadOnlyCollection<long> aggregateIds)
                {
                    return API.Specifications.Specs.Find.ByIds<Firm>(aggregateIds);
                }

                public static FindSpecification<FirmActivity> FirmActivities(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<FirmActivity>(x => aggregateIds.Contains(x.FirmId));
                }

                public static FindSpecification<FirmBalance> FirmBalances(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<FirmBalance>(x => aggregateIds.Contains(x.FirmId));
                }

                public static FindSpecification<FirmCategory> FirmCategories(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<FirmCategory>(x => aggregateIds.Contains(x.FirmId));
                }

                public static FindSpecification<Project> Projects(IReadOnlyCollection<long> ids)
                {
                    return API.Specifications.Specs.Find.ByIds<Project>(ids);
                }

                public static FindSpecification<ProjectCategory> ProjectCategories(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<ProjectCategory>(x => aggregateIds.Contains(x.ProjectId));
                }

                public static FindSpecification<Territory> Territories(IReadOnlyCollection<long> ids)
                {
                    return API.Specifications.Specs.Find.ByIds<Territory>(ids);
                }
            }
        }
    }
}