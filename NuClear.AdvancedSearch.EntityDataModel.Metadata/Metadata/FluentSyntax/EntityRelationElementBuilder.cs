using System;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityRelationElementBuilder : MetadataElementBuilder<EntityRelationElementBuilder, EntityRelationElement>
    {
        private string _name;

        public EntityRelationElementBuilder DirectTo(EntityElement entity)
        {
            Childs(entity);
            return this;
        }

        public EntityRelationElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public EntityRelationElementBuilder AsOneOptionally()
        {
            return AddCardinality(EntityRelationCardinality.OptionalOne);
        }

        public EntityRelationElementBuilder AsOne()
        {
            return AddCardinality(EntityRelationCardinality.One);
        }

        public EntityRelationElementBuilder AsMany()
        {
            return AddCardinality(EntityRelationCardinality.Many);
        }

        protected override EntityRelationElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The relation name was not specified.");
            }

            return new EntityRelationElement(_name.AsRelativeUri().AsIdentity(), Features);
        }

        private EntityRelationElementBuilder AddCardinality(EntityRelationCardinality cardinality)
        {
            AddFeatures(new EntityRelationCardinalityFeature(cardinality));
            return this;
        }
    }
}