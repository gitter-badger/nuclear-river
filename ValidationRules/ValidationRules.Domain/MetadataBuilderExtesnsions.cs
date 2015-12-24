using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata;
using NuClear.AdvancedSearch.Common.Metadata.Builders;
using NuClear.AdvancedSearch.Common.Metadata.Features;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.AdvancedSearch.Common.Metadata.Model.Operations;
using NuClear.Storage.API.Readings;
using NuClear.Storage.API.Specifications;

namespace NuClear.ValidationRules.Domain
{
    public static class MetadataBuilderExtesnsions
    {
        public static ImportStatisticsMetadataBuilder<T, TDto> FakeOperationsProvider<T, TDto>(this ImportStatisticsMetadataBuilder<T, TDto> builder)
        {
            Func<TDto, IReadOnlyCollection<IOperation>> projector = x => new IOperation[0];
            var specification = new MapSpecification<TDto, IReadOnlyCollection<IOperation>>(projector);
            var feature = new MapSpecificationFeature<TDto, IReadOnlyCollection<IOperation>>(specification);
            return builder.WithFeatures(feature);
        }
    }
}