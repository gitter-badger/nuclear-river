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
            Name = name;
            Identity = new Uri(name, UriKind.Relative).AsIdentity();
        }

        public string Name { get; }

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
                           .ApplyMutations(Features.OfType<MutationFeature>())
                           .GroupBy(x => x.GetType())
                           .ToDictionary(x => x.Key, x => (IReadOnlyCollection<object>)x.ToArray());
        }

        public bool IsIgnored
            => Features.OfType<IgnoreFeature>().Any();

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

        public sealed class MutationFeature : IMetadataFeature
        {
            private MutationFeature(Func<IEnumerable<object>, IEnumerable<object>> action)
            {
                Action = action;
            }

            public Func<IEnumerable<object>, IEnumerable<object>> Action { get; }

            public static MutationFeature Update<T>(Func<T, bool> func, Action<T> action) where T : class
            {
                var x = new UpdateImplementation<T>(func, action);
                return new MutationFeature(x.Update);
            }

            public static MutationFeature Delete<T>(Func<T, bool> func) where T : class
            {
                var x = new DeleteImplementation<T>(func);
                return new MutationFeature(x.Delete);
            }

            private class UpdateImplementation<T> where T : class 
            {
                private readonly Func<T, bool> _func;
                private readonly Action<T> _action;

                public UpdateImplementation(Func<T, bool> func, Action<T> action)
                {
                    _func = func;
                    _action = action;
                }

                public IEnumerable<object> Update(IEnumerable<object> enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        T castedItem = item as T;
                        if (castedItem != null && _func.Invoke(castedItem))
                        {
                            _action.Invoke(castedItem);
                        }

                        yield return item;
                    }
                }
            }

            private class DeleteImplementation<T> where T : class
            {
                private readonly Func<T, bool> _func;

                public DeleteImplementation(Func<T, bool> func)
                {
                    _func = func;
                }

                public IEnumerable<object> Delete(IEnumerable<object> enumerable)
                {
                    return from object item in enumerable
                           let castedItem = item as T
                           where castedItem == null || !_func.Invoke(castedItem)
                           select item;
                }
            }
        }

        public sealed class IgnoreFeature : IUniqueMetadataFeature
        {
        }
    }
}