using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal interface IEntityInfo : IIdentifiableInfo
    {
        IEnumerable<long> QueryIdsByParentIds(ICustomerIntelligenceContext context, IReadOnlyCollection<long> parentIds);
    }
}