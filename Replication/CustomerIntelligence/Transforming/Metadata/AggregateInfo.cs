using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming.Metadata
{
    internal sealed class AggregateInfo<T> : IdentifiableInfo<T>, IAggregateInfo where T : class, IIdentifiable
    {
        private readonly IEnumerable<IValueObjectInfo> _valueObjects;

        public AggregateInfo(
            Func<IEnumerable<long>, MapSpecification<IQuery, IQueryable<T>>> mapToSourceSpecProvider,
            Func<IEnumerable<long>, MapSpecification<IQuery, IQueryable<T>>> mapToTargetSpecProvider,
            IEnumerable<IValueObjectInfo> valueObjects = null)
            : base(mapToSourceSpecProvider, mapToTargetSpecProvider)
        {
            _valueObjects = valueObjects ?? Enumerable.Empty<IValueObjectInfo>();
        }

        public IEnumerable<IValueObjectInfo> ValueObjects
        {
            get { return _valueObjects; }
        }
    }
}