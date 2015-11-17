using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace NuClear.DataTest.Metamodel.Dsl
{
    public static class ArrangeMetadataElementExtensions
    {
        internal static IEnumerable<object> ApplyMutations(this IEnumerable<object> objects, IEnumerable<ArrangeMetadataElement.MutationFeature> mutations)
        {
            return mutations.Aggregate(objects, (current, mutationFeature) => mutationFeature.Action.Invoke(current));
        }

        public static ArrangeMetadataElementBuilder Mutate(this ArrangeMetadataElementBuilder element, params Action<Mutation>[] actions)
        {
            var mutation = actions.Aggregate(new Mutation(), (accumulator, action) => { action.Invoke(accumulator); return accumulator; });
            return element.WithFeatures(mutation.Features.ToArray());
        }

        public static ArrangeMetadataElement Mutate(this ArrangeMetadataElement element, params Action<Mutation>[] actions)
        {
            var mutation = actions.Aggregate(new Mutation(), (accumulator, action) => { action.Invoke(accumulator); return accumulator; });
            return new ArrangeMetadataElement(element.Name, element.Features.Concat(mutation.Features));
        }

        public class Mutation
        {
            private readonly IList<IMetadataFeature> _features = new List<IMetadataFeature>();

            public IEnumerable<IMetadataFeature> Features => _features;

            public Mutation Update<T>(Func<T, bool> func, Action<T> action) where T : class
            {
                _features.Add(ArrangeMetadataElement.MutationFeature.Update(func, action));
                return this;
            }

            public Mutation Delete<T>(Func<T, bool> func) where T : class
            {
                _features.Add(ArrangeMetadataElement.MutationFeature.Delete(func));
                return this;
            }
        }
    }
}