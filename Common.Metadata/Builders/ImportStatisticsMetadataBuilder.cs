using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Elements;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Metamodeling.Elements;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Builders
{
    public class ImportStatisticsMetadataBuilder<T> : MetadataElementBuilder<ImportStatisticsMetadataBuilder<T>, ImportStatisticsMetadata<T>>
    {
        private Type _statisticsDtoType;
        private Func<long, FindSpecification<T>> _findSpecificationProvider;
        private MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> _mapSpecification;

        protected override ImportStatisticsMetadata<T> Create()
        {
            return new ImportStatisticsMetadata<T>(_statisticsDtoType, _findSpecificationProvider, _mapSpecification, Features);
        }

        public ImportStatisticsMetadataBuilder<T> HasSource<TStatisticsDto>(MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> mapSpecification)
        {
            _statisticsDtoType = typeof(TStatisticsDto);
            _mapSpecification = mapSpecification;
            return this;
        }

        public ImportStatisticsMetadataBuilder<T> Aggregated(Func<long, FindSpecification<T>> findSpecificationProvider)
        {
            _findSpecificationProvider = findSpecificationProvider;
            return this;
        }
    }
}