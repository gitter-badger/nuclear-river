using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal class ValueObjectInfo
    {
        public ValueObjectInfo(Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> query)
        {
            Query = query;
        }

        public Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> Query { get; private set; }
    }
}