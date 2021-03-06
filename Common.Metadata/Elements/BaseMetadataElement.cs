﻿using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.AdvancedSearch.Common.Metadata.Elements
{
    public abstract class BaseMetadataElement<TElement, TBuilder> : MetadataElement<TElement, TBuilder>
        where TElement : MetadataElement<TElement, TBuilder> 
        where TBuilder : MetadataElementBuilder<TBuilder, TElement>, new()
    {
        private IMetadataElementIdentity _identity;

        protected BaseMetadataElement(IMetadataElementIdentity identity, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = identity;
        }

        public override IMetadataElementIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }

        protected TFeature LookupFeature<TFeature>() where TFeature : IMetadataFeature
        {
            return Features.OfType<TFeature>().FirstOrDefault();
        }

        protected TResult ResolveFeature<TFeature, TResult>(Func<TFeature, TResult> projector, TResult defValue = default(TResult)) where TFeature : IMetadataFeature
        {
            return ResolveFeature(projector, () => defValue);
        }

        protected TResult ResolveFeature<TFeature, TResult>(Func<TFeature, TResult> projector, Func<TResult> getDefault) where TFeature : IMetadataFeature
        {
            if (projector == null)
            {
                throw new ArgumentNullException("projector");
            }
            if (getDefault == null)
            {
                throw new ArgumentNullException("getDefault");
            }
            var feature = LookupFeature<TFeature>();
            return feature == null ? getDefault() : projector(feature);
        }
    }
}