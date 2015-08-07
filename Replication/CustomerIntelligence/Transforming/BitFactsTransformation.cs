using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = CustomerIntelligence.Model;

    public sealed class BitFactsTransformation
    {
        private readonly IQuery _query;
        private readonly IDataMapper _mapper;
        private readonly ITransactionManager _transactionManager;

        public BitFactsTransformation(IQuery query, IDataMapper mapper, ITransactionManager transactionManager)
        {
            _query = query;
            _mapper = mapper;
            _transactionManager = transactionManager;
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

            var firmsBefore = _query.For(Specs.Find.FirmStatistics.ByProjectId(dto.ProjectId)).Select(stat => stat.FirmId).Distinct().ToArray();

            _mapper.DeleteAll(_query.For(Specs.Find.FirmStatistics.ByProjectId(dto.ProjectId)));
            _mapper.InsertAll<FirmCategoryStatistics>(firmCategoryStatistics.AsQueryable());

            var firmsAfter = firmCategoryStatistics.Where(stat => stat.ProjectId == dto.ProjectId).Select(stat => stat.FirmId).Distinct().ToArray();

            return firmsBefore.Union(firmsAfter).Select(id => new RecalculateAggregate(typeof(CI.Firm), id));
        }

        private IEnumerable<AggregateOperation> DoTransform(CategoryStatisticsDto dto)
        {
            var projectCategoryStatistics = dto.ToProjectCategoryStatistics();

            _mapper.DeleteAll(_query.For(Specs.Find.ProjectStatistics.ByProjectId(dto.ProjectId)));
            _mapper.InsertAll<ProjectCategoryStatistics>(projectCategoryStatistics.AsQueryable());

            return new[] { new RecalculateAggregate(typeof(CI.Project), dto.ProjectId) };
        }
    }
}
