﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.Common.Metadata.Builders;
using NuClear.AdvancedSearch.Common.Metadata.Features;
using NuClear.AdvancedSearch.Common.Metadata.Model;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Storage.API.Specifications;

namespace NuClear.AdvancedSearch.Common.Metadata.Elements
{
    public class ImportStatisticsMetadata<T, TDto> : MetadataElement<ImportStatisticsMetadata<T, TDto>, ImportStatisticsMetadataBuilder<T, TDto>>
    {
        private readonly Func<TDto, FindSpecification<T>> _findSpecificationProvider;
        private readonly IMapSpecification<TDto, IReadOnlyCollection<T>> _mapSpecification;

        private IMetadataElementIdentity _identity;

        public ImportStatisticsMetadata(
            Type statisticsDtoType,
            Func<TDto, FindSpecification<T>> findSpecificationProvider,
            IMapSpecification<TDto, IReadOnlyCollection<T>> mapSpecification,
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
            => _identity;

        public Func<TDto, FindSpecification<T>> FindSpecificationProvider
            => _findSpecificationProvider;

        public IMapSpecification<TDto, IReadOnlyCollection<T>> MapSpecification
            => _mapSpecification;

        public IMapSpecification<TDto, IReadOnlyCollection<IOperation>> RecalculationSpecification
            => Features.OfType<MapSpecificationFeature<TDto, IReadOnlyCollection<IOperation>>>().Single().MapSpecificationProvider;
    }
}