using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Replication.CustomerIntelligence.Data.Context;

namespace NuClear.AdvancedSearch.Replication.CustomerIntelligence.Transforming
{
    internal abstract class AggregateInfo
    {
        public static AggregateInfo Create<T>(
            Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> query,
            IEnumerable<EntityInfo> entities = null,
            IEnumerable<ValueObjectInfo> valueObjects = null)
        {
            return new AggregateInfoImpl<T>(query, entities, valueObjects);
        }

        public abstract Type AggregateType { get; }

        public abstract Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> Query { get; }

        public abstract IEnumerable<EntityInfo> Entities { get; }

        public abstract IEnumerable<ValueObjectInfo> ValueObjects { get; }

        private class AggregateInfoImpl<T> : AggregateInfo
        {
            private readonly Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> _query;
            private readonly IEnumerable<EntityInfo> _entities;
            private readonly IEnumerable<ValueObjectInfo> _valueObjects;

            public AggregateInfoImpl(
                Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> query,
                IEnumerable<EntityInfo> entities = null,
                IEnumerable<ValueObjectInfo> valueObjects = null)
            {
                _query = query;
                _entities = entities ?? Enumerable.Empty<EntityInfo>();
                _valueObjects = valueObjects ?? Enumerable.Empty<ValueObjectInfo>();
            }

            public override Type AggregateType
            {
                get { return typeof(T); }
            }

            public override Func<ICustomerIntelligenceContext, IEnumerable<long>, IQueryable> Query { get { return _query; } }

            public override IEnumerable<EntityInfo> Entities { get { return _entities; } }

            public override IEnumerable<ValueObjectInfo> ValueObjects { get { return _valueObjects; } }
        }
    }
}