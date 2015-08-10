using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;
using NuClear.AdvancedSearch.Replication.Data;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{

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
            _mapper.DeleteAll(_bitFactsContext.FirmStatistics.Where(stat => stat.ProjectId == dto.ProjectId));

            var firmCategoryStatistics = dto.ToFirmCategoryStatistics();
            _mapper.InsertAll(firmCategoryStatistics.AsQueryable());

            return new[] { new CalculateStatisticsOperation { ProjectId = dto.ProjectId } };
        }

        private IEnumerable<CalculateStatisticsOperation> DoTransform(CategoryStatisticsDto dto)
        {
            _mapper.DeleteAll(_bitFactsContext.CategoryStatistics.Where(stat => stat.ProjectId == dto.ProjectId));

            var projectCategoryStatistics = dto.ToProjectCategoryStatistics();
            _mapper.InsertAll(projectCategoryStatistics.AsQueryable());

            return new[] { new CalculateStatisticsOperation { ProjectId = dto.ProjectId } };
        }
    }
}
