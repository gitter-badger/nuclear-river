using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.DataTest.Metamodel.Dsl
{
    public sealed class ArrangeMetadataElement : MetadataElement<ArrangeMetadataElement, ArrangeMetadataElementBuilder>
    {
        public ArrangeMetadataElement(string name, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            Identity = new Uri(name, UriKind.Relative).AsIdentity();
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotSupportedException();
        }

        public override IMetadataElementIdentity Identity { get; }

        public IEnumerable<string> Contexts
            => Features.OfType<ContextStateFeature>().Select(x => x.Context).Distinct();

        public IReadOnlyDictionary<Type, IReadOnlyCollection<object>> GetData(string context)
        {
            return Features.OfType<ContextStateFeature>()
                           .Where(x => string.Equals(x.Context, context))
                           .SelectMany(x => x.Data)
                           .GroupBy(x => x.GetType())
                           .ToDictionary(x => x.Key, x => (IReadOnlyCollection<object>)x.ToArray());
        }

        public sealed class ContextStateFeature : IMetadataFeature
        {
            public ContextStateFeature(string context, object[] data)
            {
                Context = context;
                Data = data;
            }

            public string Context { get; }
            public object[] Data { get; }
        }
    }
}