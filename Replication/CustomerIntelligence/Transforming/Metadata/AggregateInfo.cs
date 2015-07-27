using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal sealed class AggregateInfo<T> : IdentifiableInfo<T>, IAggregateInfo
    {
        private readonly IEnumerable<IEntityInfo> _entities;
        private readonly IEnumerable<IValueObjectInfo> _valueObjects;

        public AggregateInfo(
            Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable<T>> queryByIds,
            IEnumerable<IEntityInfo> entities = null,
            IEnumerable<IValueObjectInfo> valueObjects = null) : base(queryByIds)
        {
            _entities = entities ?? Enumerable.Empty<IEntityInfo>();
            _valueObjects = valueObjects ?? Enumerable.Empty<IValueObjectInfo>();
        }

        public IEnumerable<IEntityInfo> Entities { get { return _entities; } }
        public IEnumerable<IValueObjectInfo> ValueObjects { get { return _valueObjects; } }
    }
}