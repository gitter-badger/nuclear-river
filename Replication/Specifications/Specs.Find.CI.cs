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
                public static FindSpecification<CI::ClientContact> ClientContacts(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<CI::ClientContact>(x => aggregateIds.Contains(x.ClientId));
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

                public static FindSpecification<CI::ProjectCategory> ProjectCategories(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<CI::ProjectCategory>(x => aggregateIds.Contains(x.ProjectId));
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