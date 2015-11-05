using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.DataTest.Metamodel.Dsl
{
    public sealed class ActMetadataElement : MetadataElement<ActMetadataElement, ActMetadataElementBuilder>
    {
        private IMetadataElementIdentity _identity;

        public ActMetadataElement(IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            var sourceContexts = string.Join(",", features.OfType<SourceContextFeature>().OrderBy(x => x.Context).Select(x => x.Context));
            var destinationContexts = string.Join(",", features.OfType<TargetContextFeature>().OrderBy(x => x.Context).Select(x => x.Context));

            _identity = new Uri(Path.Combine($"From[{sourceContexts}]", $"To[{destinationContexts}]"), UriKind.Relative).AsIdentity();
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotSupportedException();
        }

        public override IMetadataElementIdentity Identity
            => _identity;

        public IReadOnlyCollection<string> Requirements
            => Features.OfType<RequiredContextFeature>().Select(x => x.Context).ToArray();

        public string Source
            => Features.OfType<SourceContextFeature>().Select(x => x.Context).Single();

        public string Target
            => Features.OfType<TargetContextFeature>().Select(x => x.Context).Single();

        public IEnumerable<Type> ActionTypes
            => Features.OfType<ActionFeature>().OrderBy(x => x.Order).Select(x => x.ActionType);

        public class RequiredContextFeature : IMetadataFeature
        {
            public RequiredContextFeature(string context)
            {
                Context = context;
            }

            public string Context { get; }
        }

        public class SourceContextFeature : IUniqueMetadataFeature
        {
            public SourceContextFeature(string context)
            {
                Context = context;
            }

            public string Context { get; }
        }

        public class TargetContextFeature : IUniqueMetadataFeature
        {
            public TargetContextFeature(string context)
            {
                Context = context;
            }

            public string Context { get; }
        }

        public class ActionFeature : IMetadataFeature
        {
            public ActionFeature(int order, Type actionType)
            {
                Order = order;
                ActionType = actionType;
            }

            public  int Order { get; }

            public Type ActionType { get; }
        }
    }
}