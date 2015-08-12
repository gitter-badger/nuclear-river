using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Operations;
using NuClear.AdvancedSearch.Replication.Data;
using NuClear.AdvancedSearch.Replication.Specifications;
using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{

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

        public IEnumerable<CalculateStatisticsOperation> Transform(FirmStatisticsDto dto)
        {
            return _transactionManager.WithinTransaction(() => DoTransform(dto));
        }

        public IEnumerable<CalculateStatisticsOperation> Transform(CategoryStatisticsDto dto)
        {
            return _transactionManager.WithinTransaction(() => DoTransform(dto));
        }

        private IEnumerable<CalculateStatisticsOperation> DoTransform(FirmStatisticsDto dto)
        {
            _mapper.DeleteAll(_query.For(Specs.Find.FirmStatistics.ByProjectId(dto.ProjectId)));

            var firmCategoryStatistics = dto.ToFirmCategoryStatistics();
            _mapper.InsertAll(firmCategoryStatistics.AsQueryable());

            return new[] { new CalculateStatisticsOperation { ProjectId = dto.ProjectId } };
        }

        private IEnumerable<CalculateStatisticsOperation> DoTransform(CategoryStatisticsDto dto)
        {
            _mapper.DeleteAll(_query.For(Specs.Find.ProjectStatistics.ByProjectId(dto.ProjectId)));

            var projectCategoryStatistics = dto.ToProjectCategoryStatistics();
            _mapper.InsertAll(projectCategoryStatistics.AsQueryable());

            return new[] { new CalculateStatisticsOperation { ProjectId = dto.ProjectId } };
        }
    }
}
