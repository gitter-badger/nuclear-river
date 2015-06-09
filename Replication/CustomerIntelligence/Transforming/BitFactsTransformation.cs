using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context.Implementation;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = CustomerIntelligence.Model;

    public sealed class BitFactsTransformation
    {
        private readonly IDataMapper _mapper;
        private readonly IBitFactsContext _bitFactsContext;

        public BitFactsTransformation(IBitFactsContext bitFactsContext, IDataMapper mapper)
        {
            _mapper = mapper;
            _bitFactsContext = bitFactsContext;
        }

        public IEnumerable<AggregateOperation> Transform(FirmStatisticsDto dto)
        {
            var firmCategoryStatistics = dto.ToFirmCategoryStatistics();

            var firmsBefore = _bitFactsContext.FirmStatistics.Where(stat => stat.ProjectId == dto.ProjectId).Select(stat => stat.FirmId).Distinct().ToArray();
            
            _mapper.DeleteAll(_bitFactsContext.FirmStatistics);
            _mapper.InsertAll(firmCategoryStatistics.AsQueryable());

            var firmsAfter = firmCategoryStatistics.Where(stat => stat.ProjectId == dto.ProjectId).Select(stat => stat.FirmId).Distinct().ToArray();

            return firmsBefore.Union(firmsAfter).Select(id => new RecalculateAggregate(typeof(CI.Firm), id));
        }

        public IEnumerable<AggregateOperation> Transform(CategoryStatisticsDto dto)
        {
            var projectCategoryStatistics = dto.ToProjectCategoryStatistics();

            _mapper.DeleteAll(_bitFactsContext.CategoryStatistics);
            _mapper.InsertAll(projectCategoryStatistics.AsQueryable());

            return new [] { new RecalculateAggregate(typeof(CI.Project), dto.ProjectId) };
        }
    }
}
