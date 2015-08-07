using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.Specifications
{
    public static partial class Specs
    {
        public static class Find
        {
            public static FindSpecification<T> ByIds<T>(IEnumerable<long> ids) where T : IIdentifiable
            {
                return new FindSpecification<T>(x => ids.Contains(x.Id));
            }

            public static FindSpecification<long> ByIds(IEnumerable<long> ids)
            {
                return new FindSpecification<long>(x => ids.Contains(x));
            }

            public static class FirmStatistics
            {
                public static FindSpecification<FirmCategoryStatistics> ByProjectId(long projectId)
                {
                    return new FindSpecification<FirmCategoryStatistics>(x => x.ProjectId == projectId);
                }
            }

            public static class ProjectStatistics
            {
                public static FindSpecification<ProjectCategoryStatistics> ByProjectId(long projectId)
                {
                    return new FindSpecification<ProjectCategoryStatistics>(x => x.ProjectId == projectId);
                }
            }
        }
    }
}