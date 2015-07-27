using System.Collections;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal interface IIdentifiableInfo : IMetadataInfo
    {
        IEnumerable QueryByIds(ICustomerIntelligenceContext context, IReadOnlyCollection<long> ids);
        IEnumerable<long> QueryIdsByIds(ICustomerIntelligenceContext context, IReadOnlyCollection<long> ids);
    }
}