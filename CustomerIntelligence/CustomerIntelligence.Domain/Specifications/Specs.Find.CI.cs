using System.Collections.Generic;
using System.Linq;

using NuClear.CustomerIntelligence.Domain.Model.CI;
using NuClear.Storage.API.Specifications;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    public static partial class Specs
    {
        public static partial class Find
        {
            public static partial class CI
            {
                public static FindSpecification<ClientContact> ClientContacts(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<ClientContact>(x => aggregateIds.Contains(x.ClientId));
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

                public static FindSpecification<FirmTerritory> FirmTerritories(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<FirmTerritory>(x => aggregateIds.Contains(x.FirmId));
                }

                public static FindSpecification<ProjectCategory> ProjectCategories(IReadOnlyCollection<long> aggregateIds)
                {
                    return new FindSpecification<ProjectCategory>(x => aggregateIds.Contains(x.ProjectId));
                }

                public static partial class FirmCategoryStatistics
                {
                    public static FindSpecification<Model.CI.FirmCategoryStatistics> ByProject(long projectId)
                    {
                        return new FindSpecification<Model.CI.FirmCategoryStatistics>(x => x.ProjectId == projectId);
                    }

                    public static FindSpecification<Model.CI.FirmCategoryStatistics> ByProjectAndCategories(long projectId, IReadOnlyCollection<long?> categoryIds)
                    {
                        return new FindSpecification<Model.CI.FirmCategoryStatistics>(x => x.ProjectId == projectId && categoryIds.Contains(x.CategoryId));
                    }
                }
            }
        }
    }
}