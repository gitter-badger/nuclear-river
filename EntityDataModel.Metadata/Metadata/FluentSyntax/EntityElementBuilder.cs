using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityElementBuilder : MetadataElementBuilder<EntityElementBuilder, EntityElement>
    {
        private readonly HashSet<Uri> _keyNames = new HashSet<Uri>();
        private readonly List<EntityPropertyElementBuilder> _properties = new List<EntityPropertyElementBuilder>();
        private readonly List<EntityRelationElementBuilder> _relations = new List<EntityRelationElementBuilder>();

        private string _name;
        private string _entitySetName;

        public string EntityName
        {
            get
            {
                return _name;
            }
        }

        public IReadOnlyCollection<EntityPropertyElementBuilder> Properties
        {
            get
            {
                return _properties;
            }
        }

        public IReadOnlyCollection<EntityRelationElementBuilder> Relations
        {
            get
            {
                return _relations;
            }
        }

        public EntityElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public EntityElementBuilder EntitySetName(string entitySetName)
        {
            _entitySetName = entitySetName;
            return this;
        }

        public EntityElementBuilder HasKey(params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                _keyNames.Add(propertyName.AsUri());
            }
            return this;
        }

        public EntityElementBuilder Property(EntityPropertyElementBuilder property)
        {
            _properties.Add(property);
            return this;
        }

        public EntityElementBuilder Relation(EntityRelationElementBuilder relation)
        {
            _relations.Add(relation);
            return this;
        }

        protected override EntityElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The entity name was not specified.");
            }

            if (!string.IsNullOrWhiteSpace(_entitySetName))
            {
                AddFeatures(new EntitySetFeature(_entitySetName));
            }

            ProcessProperties();
            ProcessRelations();

            return new EntityElement(_name.AsUri().AsIdentity(), Features);
        }

        private void ProcessProperties()
        {
            var keys = new List<EntityPropertyElement>();

            foreach (var propertyElement in _properties.Select(x => (EntityPropertyElement)x))
            {
                Childs(propertyElement);

                if (_keyNames.Contains(propertyElement.Identity.Id))
                {
                    keys.Add(propertyElement);
                }
            }

            if (keys.Count > 0)
            {
                AddFeatures(new EntityIdentityFeature(keys));
            }
        }

        private void ProcessRelations()
        {
            Childs(_relations.Select(x => (IMetadataElement)(EntityRelationElement)x).ToArray());
        }
    }
}