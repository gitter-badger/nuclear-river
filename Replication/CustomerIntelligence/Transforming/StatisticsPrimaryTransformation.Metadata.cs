using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;
using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Operations;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    using CI = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model;
    using Facts = NuClear.AdvancedSearch.Replication.CustomerIntelligence.Model.Facts;

    public partial class StatisticsPrimaryTransformation
    {

        private static readonly IDictionary<Type, Query> Transformations =
            new Dictionary<Type, Query>
            {
                { typeof(Facts.Firm), ByFirm },
                { typeof(Facts.FirmAddress), ByFirmAddress },
                { typeof(Facts.CategoryFirmAddress), ByFirmAddressCategory },
                { typeof(Facts.Project), ByProject },
            };

        private delegate IEnumerable<CalculateStatisticsOperation> Query(IErmFactsContext context, IEnumerable<long> ids);

        private static IEnumerable<CalculateStatisticsOperation> ByFirm(IErmFactsContext context, IEnumerable<long> ids)
        {
            return from firm in context.Firms.Where(x => ids.Contains(x.Id))
                   join project in context.Projects on firm.OrganizationUnitId equals project.OrganizationUnitId
                   join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                   join firmAddressCategory in context.CategoryFirmAddresses on firmAddress.Id equals firmAddressCategory.FirmAddressId
                   where ids.Contains(firm.Id)
                   select new CalculateStatisticsOperation { CategoryId = firmAddressCategory.CategoryId, ProjectId = project.Id };
        }

        private static IEnumerable<CalculateStatisticsOperation> ByFirmAddress(IErmFactsContext context, IEnumerable<long> ids)
        {
            return from firm in context.Firms
                   join project in context.Projects on firm.OrganizationUnitId equals project.OrganizationUnitId
                   join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                   join firmAddressCategory in context.CategoryFirmAddresses on firmAddress.Id equals firmAddressCategory.FirmAddressId
                   where ids.Contains(firmAddress.Id)
                   select new CalculateStatisticsOperation { CategoryId = firmAddressCategory.CategoryId, ProjectId = project.Id };
        }

        private static IEnumerable<CalculateStatisticsOperation> ByFirmAddressCategory(IErmFactsContext context, IEnumerable<long> ids)
        {
            return from firm in context.Firms
                   join project in context.Projects on firm.OrganizationUnitId equals project.OrganizationUnitId
                   join firmAddress in context.FirmAddresses on firm.Id equals firmAddress.FirmId
                   join firmAddressCategory in context.CategoryFirmAddresses on firmAddress.Id equals firmAddressCategory.FirmAddressId
                   where ids.Contains(firmAddressCategory.Id)
                   select new CalculateStatisticsOperation { CategoryId = firmAddressCategory.CategoryId, ProjectId = project.Id };
        }

        private static IEnumerable<CalculateStatisticsOperation> ByProject(IErmFactsContext context, IEnumerable<long> ids)
        {
            return from projectId in ids
                   select new CalculateStatisticsOperation { CategoryId = null, ProjectId = projectId };
        }

    }
}
