using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal sealed class AggregateInfo<T> : IdentifiableInfo<T>, IAggregateInfo
    {
        private readonly IEnumerable<IValueObjectInfo> _valueObjects;

        public AggregateInfo(
            Func<IQuery, IEnumerable<long>, IQueryable<T>> queryByIds,
            IEnumerable<IValueObjectInfo> valueObjects = null) : base(queryByIds)
        {
            _valueObjects = valueObjects ?? Enumerable.Empty<IValueObjectInfo>();
        }

        public IEnumerable<IValueObjectInfo> ValueObjects { get { return _valueObjects; } }
    }
}