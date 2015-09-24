using NuClear.Storage.Specifications;

using Bit = NuClear.CustomerIntelligence.Domain.Model.Facts;

namespace NuClear.CustomerIntelligence.Domain.Specifications
{
    public static partial class Specs
    {
        public static partial class Find
        {
            public static partial class Bit
            {
                public static partial class FirmCategoryStatistics
                {
                    public static FindSpecification<Bit::FirmCategoryStatistics> ByProject(long projectId)
                    {
                        return new FindSpecification<Bit::FirmCategoryStatistics>(x => x.ProjectId == projectId);
                    }
                }

                public static class ProjectCategoryStatistics
                {
                    public static FindSpecification<Bit::ProjectCategoryStatistics> ByProject(long projectId)
                    {
                        return new FindSpecification<Bit::ProjectCategoryStatistics>(x => x.ProjectId == projectId);
                    }
                }
            }
        }
    }
}