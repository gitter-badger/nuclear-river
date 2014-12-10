using System;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.EntityDataModel
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

        public EntityRelationElementBuilder AsSingle()
        {
            return AddCardinality(Cardinality.One);
        }

        public EntityRelationElementBuilder AsSingleOptionally()
        {
            return AddCardinality(Cardinality.OptionalOne);
        }

        public EntityRelationElementBuilder AsMany()
        {
            return AddCardinality(Cardinality.Many);
        }

        public EntityRelationElementBuilder AsManyOptionally()
        {
            return AddCardinality(Cardinality.OptionalMany);
        }

        private EntityRelationElementBuilder AddCardinality(Cardinality cardinality)
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