using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming
{
    internal sealed class ValueObjectInfo<TValueObject> : IMetadataInfo
    {
        private readonly Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TValueObject>>> _mapToSourceSpecProvider;
        private readonly Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TValueObject>>> _mapToTargetSpecProvider;

        public ValueObjectInfo(
            Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TValueObject>>> mapToSourceSpecProvider,
            Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TValueObject>>> mapToTargetSpecProvider)
        {
            _mapToSourceSpecProvider = mapToSourceSpecProvider;
            _mapToTargetSpecProvider = mapToTargetSpecProvider;
        }

        public Type Type
        {
            get { return typeof(TValueObject); }
        }

        public MapToObjectsSpecProvider MapToSourceSpecProvider
        {
            get
            {
                return ids =>
                {
                    if (!ids.Any())
                    {
                        return new MapSpecification<IQuery, IEnumerable>(q => Enumerable.Empty<TValueObject>());
                    }

                    var mapToSourceSpec = _mapToSourceSpecProvider(ids);
                    return new MapSpecification<IQuery, IEnumerable>(mapToSourceSpec);
                };
            }
        }

        public MapToObjectsSpecProvider MapToTargetSpecProvider
        {
            get
            {
                return ids =>
                {
                    if (!ids.Any())
                    {
                        return new MapSpecification<IQuery, IEnumerable>(q => Enumerable.Empty<TValueObject>());
                    }

                    var mapToTargetSpec = _mapToTargetSpecProvider(ids);
                    return new MapSpecification<IQuery, IEnumerable>(mapToTargetSpec);
                };
            }
        }
    }
}