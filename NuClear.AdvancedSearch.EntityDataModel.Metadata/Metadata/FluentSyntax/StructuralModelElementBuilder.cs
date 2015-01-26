using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class StructuralModelElementBuilder : MetadataElementBuilder<StructuralModelElementBuilder, StructuralModelElement>
    {
        private string _name;
        private IEnumerable<EntityElementBuilder> _elements = Enumerable.Empty<EntityElementBuilder>();

        public StructuralModelElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public StructuralModelElementBuilder Elements(params EntityElementBuilder[] elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            _elements = _elements.Concat(elements);
            return this;
        }

        protected override StructuralModelElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The model name was not specified.");
            }

            IEnumerable<EntityElement> rootEntities;
            IEnumerable<EntityElement> allEntities;
            ProcessElements(_elements, out rootEntities, out allEntities);

            Childs(allEntities.ToArray());

            return new StructuralModelElement(_name.AsUri().AsIdentity(), rootEntities, Features);
        }

        private static void ProcessElements(IEnumerable<EntityElementBuilder> elements, out IEnumerable<EntityElement> roots, out IEnumerable<EntityElement> entities)
        {
            var rootEntities = new List<EntityElement>();
            var entityById = new Dictionary<Uri, EntityElement>();

            foreach (var rootElement in elements)
            {
                foreach (var relationElement in EnumerateRelation(rootElement).Where(x => x.Target != null))
                {
                    var targetId = relationElement.Target.EntityName.AsUri();
                    var target = AddIfNotPresented(entityById, targetId, relationElement.Target);
                    relationElement.DirectTo(target);
                }

                var rootId = rootElement.EntityName.AsUri();
                rootEntities.Add(AddIfNotPresented(entityById, rootId, rootElement));
            }

            roots = rootEntities;
            entities = entityById.Values;
        }

        private static EntityElement AddIfNotPresented(IDictionary<Uri, EntityElement> dictionary, Uri entityId, EntityElementBuilder entityConfig)
        {
            if (!dictionary.ContainsKey(entityId))
            {
                dictionary.Add(entityId, entityConfig);
            }
            return dictionary[entityId];
        }

        private static IEnumerable<EntityRelationElementBuilder> EnumerateRelation(EntityElementBuilder entityConfig)
        {
            foreach (var relation in entityConfig.Relations)
            {
                foreach (var nestedRelation in EnumerateRelation(relation.Target))
                {
                    yield return nestedRelation;
                }
                yield return relation;
            }
        }
    }
}