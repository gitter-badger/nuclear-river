using System;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

namespace NuClear.EntityDataModel
{
    public sealed class EntityElementBuilder : MetadataElementBuilder<EntityElementBuilder, EntityElement>
    {
        private string _name;

        public EntityElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        internal EntityElementBuilder Property(EntityPropertyElement property)
        {
            Childs(property);
            return this;
        }

        internal EntityElementBuilder Relation(EntityRelationElement relation)
        {
            Childs(relation);
            return this;
        }

        protected override EntityElement Create()
        {
            return new EntityElement(new Uri(_name ?? "", UriKind.Relative).AsIdentity(), Features);
        }
    }
}