using System.Collections;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal interface IValueObjectInfo : IMetadataInfo
    {
        IEnumerable QueryByParentIds(ICustomerIntelligenceContext context, IReadOnlyCollection<long> parentIds);
    }
}