using System;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityRelationElementBuilder : MetadataElementBuilder<EntityRelationElementBuilder, EntityRelationElement>
    {
        private string _name;
        private EntityRelationCardinality? _cardinality;
        private EntityElement _targetEntity;
        private EntityElementBuilder _targetEntityConfig;

        public EntityElementBuilder Target
        {
            get
            {
                return _targetEntityConfig;
            }
        }

        public EntityRelationElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public EntityRelationElementBuilder DirectTo(EntityElement entity)
        {
            _targetEntity = entity;
            return this;
        }

        public EntityRelationElementBuilder DirectTo(EntityElementBuilder entityConfig)
        {
            _targetEntityConfig = entityConfig;
            return this;
        }

        public EntityRelationElementBuilder AsOneOptionally()
        {
            _cardinality = EntityRelationCardinality.OptionalOne;
            return this;
        }

        public EntityRelationElementBuilder AsOne()
        {
            _cardinality = EntityRelationCardinality.One;
            return this;
        }

        public EntityRelationElementBuilder AsMany()
        {
            _cardinality = EntityRelationCardinality.Many;
            return this;
        }

        protected override EntityRelationElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The relation name was not specified.");
            }
            if (!_cardinality.HasValue)
            {
                throw new InvalidOperationException("The relation cardinality was not specified.");
            }
            if (_targetEntity == null && _targetEntityConfig == null)
            {
                throw new InvalidOperationException("The relation target was not specified.");
            }

            AddFeatures(new EntityRelationCardinalityFeature(_cardinality.Value, _targetEntity ?? _targetEntityConfig));

            return new EntityRelationElement(_name.AsUri().AsIdentity(), Features);
        }
    }
}