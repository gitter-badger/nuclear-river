using System;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityRelationElementBuilder : MetadataElementBuilder<EntityRelationElementBuilder, EntityRelationElement>
    {
        private string _name;
        private EntityRelationCardinality? _cardinality;
        private EntityElement _targetEntityElement;
        private EntityElementBuilder _targetEntityElementConfig;

        internal Uri TargetEntityReference
        {
            get
            {
                if (_targetEntityElement != null)
                {
                    return _targetEntityElement.Identity.Id;
                }
                if (_targetEntityElementConfig != null)
                {
                    return _targetEntityElementConfig.EntityId;
                }
                throw new InvalidOperationException("The reference is not set.");
            }
        }

        internal EntityElementBuilder TargetEntityElementConfig
        {
            get
            {
                return _targetEntityElementConfig;
            }
        }

        public EntityRelationElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public EntityRelationElementBuilder DirectTo(EntityElement entityElement)
        {
            _targetEntityElement = entityElement;
            _targetEntityElementConfig = null;
            return this;
        }

        public EntityRelationElementBuilder DirectTo(EntityElementBuilder entityElementBuilder)
        {
            _targetEntityElement = null;
            _targetEntityElementConfig = entityElementBuilder;
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
            if (_targetEntityElement == null && _targetEntityElementConfig == null)
            {
                throw new InvalidOperationException("The relation target was not specified.");
            }

            AddFeatures(new EntityRelationCardinalityFeature(_cardinality.Value, _targetEntityElement ?? _targetEntityElementConfig));

            return new EntityRelationElement(_name.AsUri().AsIdentity(), Features);
        }
    }
}