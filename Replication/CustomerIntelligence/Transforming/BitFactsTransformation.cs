using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = CustomerIntelligence.Model;

    public sealed class BitFactsTransformation
    {
        private readonly IDataMapper _mapper;
        private readonly IBitFactsContext _bitFactsContext;
        private readonly ITransactionManager _transactionManager;

        public BitFactsTransformation(IBitFactsContext bitFactsContext, IDataMapper mapper, ITransactionManager transactionManager)
        {
            _mapper = mapper;
            _transactionManager = transactionManager;
            _bitFactsContext = bitFactsContext;
        }

        public IEnumerable<AggregateOperation> Transform(FirmStatisticsDto dto)
        {
            return _transactionManager.WithinTransaction(() => DoTransform(dto));
        }

        public IEnumerable<AggregateOperation> Transform(CategoryStatisticsDto dto)
        {
            return _transactionManager.WithinTransaction(() => DoTransform(dto));
        }

        private IEnumerable<AggregateOperation> DoTransform(FirmStatisticsDto dto)
        {
            var firmCategoryStatistics = dto.ToFirmCategoryStatistics();

            var firmsBefore = _bitFactsContext.FirmStatistics.Where(stat => stat.ProjectId == dto.ProjectId).Select(stat => stat.FirmId).Distinct().ToArray();

            _mapper.DeleteAll(_bitFactsContext.FirmStatistics.Where(stat => stat.ProjectId == dto.ProjectId));
            _mapper.InsertAll<FirmCategoryStatistics>(firmCategoryStatistics.AsQueryable());

            var firmsAfter = firmCategoryStatistics.Where(stat => stat.ProjectId == dto.ProjectId).Select(stat => stat.FirmId).Distinct().ToArray();

            return firmsBefore.Union(firmsAfter).Select(id => new RecalculateAggregate(typeof(CI.Firm), id));
        }

        private IEnumerable<AggregateOperation> DoTransform(CategoryStatisticsDto dto)
        {
            var projectCategoryStatistics = dto.ToProjectCategoryStatistics();

            _mapper.DeleteAll(_bitFactsContext.CategoryStatistics.Where(stat => stat.ProjectId == dto.ProjectId));
            _mapper.InsertAll<ProjectCategoryStatistics>(projectCategoryStatistics.AsQueryable());

            return new [] { new RecalculateAggregate(typeof(CI.Project), dto.ProjectId) };
        }
    }
}
