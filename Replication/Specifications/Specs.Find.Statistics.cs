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