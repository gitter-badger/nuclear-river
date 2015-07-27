using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal sealed class ValueObjectInfo<T> : IValueObjectInfo
    {
        private readonly Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable<T>> _queryByParentIds;

        public ValueObjectInfo(Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable<T>> queryByParentIds)
        {
            _queryByParentIds = queryByParentIds;
        }

        public Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable<T>> QueryByParentIds { get { return _queryByParentIds; } }
        public Type Type { get { return typeof(T); } }
        IEnumerable IValueObjectInfo.QueryByParentIds(ICustomerIntelligenceContext context, IReadOnlyCollection<long> parentIds)
        {
            if (!parentIds.Any())
            {
                return Enumerable.Empty<long>();
            }

            return QueryByParentIds(context, parentIds);
        }
    }
}