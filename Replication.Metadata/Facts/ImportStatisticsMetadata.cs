using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Replication.Metadata.Model;
using NuClear.Storage.Specifications;

namespace NuClear.Replication.Metadata.Facts
{
    public class ImportStatisticsMetadata<T> : MetadataElement<ImportStatisticsMetadata<T>, ImportStatisticsMetadataBuilder<T>>
    {
        private readonly IMetadataElementIdentity _identity = new Uri(typeof(T).Name, UriKind.Relative).AsIdentity();

        private readonly Type _statisticsDtoType;
        private readonly Func<long, FindSpecification<T>> _findSpecificationProvider;
        private readonly MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> _mapSpecification;

        public ImportStatisticsMetadata(
            Type statisticsDtoType,
            Func<long, FindSpecification<T>> findSpecificationProvider,
            MapSpecification<IStatisticsDto, IReadOnlyCollection<T>> mapSpecification,
            IEnumerable<IMetadataFeature> features) : base(features)
        {
            _statisticsDtoType = statisticsDtoType;
            _findSpecificationProvider = findSpecificationProvider;
            _mapSpecification = mapSpecification;
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotSupportedException();
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public Type StatisticsDtoType
        {
            get { return _statisticsDtoType; }
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