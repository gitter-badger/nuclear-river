using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal sealed class EntityInfo<T> : IdentifiableInfo<T>, IEntityInfo
    {
        private readonly Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable<T>> _queryByParentIds;

        public EntityInfo(
            Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable<T>> queryByIds,
            Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable<T>> queryByParentIds) : base(queryByIds)
        {
            _queryByParentIds = queryByParentIds;
        }

        public Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable<T>> QueryByParentIds { get { return _queryByParentIds; } }

        IEnumerable<long> IEntityInfo.QueryIdsByParentIds(ICustomerIntelligenceContext context, IReadOnlyCollection<long> parentIds)
        {
            if (!parentIds.Any())
            {
                return Enumerable.Empty<long>();
            }

            var actualIds = QueryByParentIds(context, parentIds).Select(SelectIdExpression());
            return actualIds;
        }
    }
}