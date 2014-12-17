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

        public EntityRelationElementBuilder DirectTo(EntityElement entity)
        {
            Childs(entity.Elements);
            AddFeatures(entity.Features.ToArray());
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

        private EntityRelationElementBuilder AddCardinality(EntityRelationCardinality cardinality)
        {
            AddFeatures(new EntityRelationCardinalityFeature(cardinality));
            return this;
        }

        protected override EntityRelationElement Create()
        {
            return new EntityRelationElement(new Uri(_name, UriKind.Relative).AsIdentity(), Features);
        }
    }
}