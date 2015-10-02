using System;
using System.Collections.Generic;

using NuClear.AdvancedSearch.Common.Metadata.Builders;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Elements
{
    public class ImportStatisticsMetadata<T> : MetadataElement<ImportStatisticsMetadata<T>, ImportStatisticsMetadataBuilder<T>>
    {
        private readonly Func<long, FindSpecification<T>> _findSpecificationProvider;
        private readonly MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> _mapSpecification;

        private IMetadataElementIdentity _identity;

        public ImportStatisticsMetadata(
            Type statisticsDtoType,
            Func<long, FindSpecification<T>> findSpecificationProvider,
            MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> mapSpecification,
            IEnumerable<IMetadataFeature> features) : base(features)
        {
            _identity = new Uri(statisticsDtoType.Name, UriKind.Relative).AsIdentity();
            _findSpecificationProvider = findSpecificationProvider;
            _mapSpecification = mapSpecification;
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public Func<long, FindSpecification<T>> FindSpecificationProvider
        {
            get { return _findSpecificationProvider; }
        }

        public MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> MapSpecification
        {
            get { return _mapSpecification; }
        }
    }
}