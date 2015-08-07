using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal sealed class ValueObjectInfo<T> : IValueObjectInfo
    {
        private readonly Func<IQuery, IEnumerable<long>, IQueryable<T>> _queryByParentIds;

        public ValueObjectInfo(Func<IQuery, IEnumerable<long>, IQueryable<T>> queryByParentIds)
        {
            _queryByParentIds = queryByParentIds;
        }

        public Func<IQuery, IEnumerable<long>, IQueryable<T>> QueryByParentIds
        {
            get { return _queryByParentIds; }
        }

        public Type Type
        {
            get { return typeof(T); }
        }

        IEnumerable IValueObjectInfo.QueryByParentIds(IQuery context, IReadOnlyCollection<long> parentIds)
        {
            if (!parentIds.Any())
            {
                return Enumerable.Empty<long>();
            }

            return QueryByParentIds(context, parentIds);
        }
    }
}