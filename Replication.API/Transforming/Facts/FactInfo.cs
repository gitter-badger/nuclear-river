using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using NuClear.AdvancedSearch.Replication.API.Model;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

namespace NuClear.AdvancedSearch.Replication.API.Transforming.Facts
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public static class FactInfo
    {
        public static FactInfoBuilder<TFact> OfType<TFact>(params object[] x) where TFact : class, IErmFactObject, IIdentifiable
        {
            return new FactInfoBuilder<TFact>();
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class FactInfo<TFact> : IFactInfo where TFact : class, IErmFactObject
    {
        private readonly Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TFact>>> _mapToSourceSpecProvider;
        private readonly Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TFact>>> _mapToTargetSpecProvider;
        private readonly CalculateStatisticsSpecProvider _calculateStatisticsSpecProvider;
        private readonly IReadOnlyCollection<IFactDependencyInfo> _aggregates;

        public FactInfo(
            Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TFact>>> mapToSourceSpecProvider,
            Func<IReadOnlyCollection<long>, MapSpecification<IQuery, IQueryable<TFact>>> mapToTargetSpecProvider,
            CalculateStatisticsSpecProvider calculateStatisticsSpecProvider,
            IReadOnlyCollection<IFactDependencyInfo> aggregates)
        {
            _mapToSourceSpecProvider = mapToSourceSpecProvider;
            _mapToTargetSpecProvider = mapToTargetSpecProvider;
            _calculateStatisticsSpecProvider = calculateStatisticsSpecProvider;
            _aggregates = aggregates ?? new IFactDependencyInfo[0];
        }

        public Type Type
        {
            get { return typeof(TFact); }
        }

        public IReadOnlyCollection<IFactDependencyInfo> DependencyInfos
        {
            get { return _aggregates; }
        }

        public CalculateStatisticsSpecProvider CalculateStatisticsSpecProvider
        {
            get { return _calculateStatisticsSpecProvider; }
        }

        public MapToObjectsSpecProvider MapToSourceSpecProvider
        {
            get
            {
                return ids =>
                {
                    if (!ids.Any())
                    {
                        return new MapSpecification<IQuery, IEnumerable>(q => Enumerable.Empty<TFact>());
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
                        return new MapSpecification<IQuery, IEnumerable>(q => Enumerable.Empty<TFact>());
                    }

                    var mapToTargetSpec = _mapToTargetSpecProvider(ids);
                    return new MapSpecification<IQuery, IEnumerable>(mapToTargetSpec);
                };
            }
        }
    }
}