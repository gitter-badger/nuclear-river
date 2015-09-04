using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;

    public static partial class Specs
    {
        public static partial class Find
        {
            public static partial class CI
            {
                public static FindSpecification<CI::CategoryGroup> CategoryGroups(IReadOnlyCollection<long> ids)
                {
                    return API.Specifications.Specs.Find.ByIds<CI::CategoryGroup>(ids);
                }

                public static FindSpecification<CI::Client> Clients(IReadOnlyCollection<long> ids)
                {
                    return API.Specifications.Specs.Find.ByIds<CI::Client>(ids);
                }

                public static FindSpecification<CI::ClientContact> ClientContacts(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<CI::ClientContact>(x => aggregateIds.Contains(x.ClientId));
                }

                public static FindSpecification<CI::Firm> Firms(IReadOnlyCollection<long> aggregateIds)
                {
                    return API.Specifications.Specs.Find.ByIds<CI::Firm>(aggregateIds);
                }

                public static FindSpecification<CI::FirmActivity> FirmActivities(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<CI::FirmActivity>(x => aggregateIds.Contains(x.FirmId));
                }

                public static FindSpecification<CI::FirmBalance> FirmBalances(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<CI::FirmBalance>(x => aggregateIds.Contains(x.FirmId));
                }

                public static FindSpecification<CI::FirmCategory> FirmCategories(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<CI::FirmCategory>(x => aggregateIds.Contains(x.FirmId));
                }

                public static FindSpecification<CI::Project> Projects(IReadOnlyCollection<long> ids)
                {
                    return API.Specifications.Specs.Find.ByIds<CI::Project>(ids);
                }

                public static FindSpecification<CI::ProjectCategory> ProjectCategories(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<CI::ProjectCategory>(x => aggregateIds.Contains(x.ProjectId));
                }

                public static FindSpecification<CI::Territory> Territories(IReadOnlyCollection<long> ids)
                {
                    return API.Specifications.Specs.Find.ByIds<CI::Territory>(ids);
                }

                public static partial class FirmCategoryStatistics
                {
                    public static FindSpecification<CI::FirmCategoryStatistics> ByProject(long projectId)
                    {
                        return new FindSpecification<CI::FirmCategoryStatistics>(x => x.ProjectId == projectId);
                    }

                    public static FindSpecification<CI::FirmCategoryStatistics> ByProjectAndCategories(long projectId, IReadOnlyCollection<long?> categoryIds)
                    {
                        return new FindSpecification<CI::FirmCategoryStatistics>(x => x.ProjectId == projectId && categoryIds.Contains(x.CategoryId));
                    }
                }
            }
        }
    }
}