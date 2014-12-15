using System;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityElementBuilder : MetadataElementBuilder<EntityElementBuilder, EntityElement>
    {
        private string _name;

        public EntityElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public EntityElementBuilder IdentifyBy(params string[] propertyNames)
        {
            if (propertyNames.Length == 0)
            {
                throw new ArgumentException("At least one name should be specified.", "propertyNames");
            }

            var properties = ChildElements.OfType<EntityPropertyElement>().ToDictionary(x => x.Identity.Id);

            Func<string, EntityPropertyElement> resolve = propertyName =>
                {
                    EntityPropertyElement property;
                    
                    var propertyId = new Uri(propertyName, UriKind.Relative);
                    if (!properties.TryGetValue(propertyId, out property))
                    {
                        throw new ArgumentException(string.Format("The property with name: '{0}' was not declared.", propertyName), "propertyNames");
                    }
                    return property;
                };

            AddFeatures(new EntityIdentityFeature(propertyNames.Distinct().Select(propertyName => resolve(propertyName)).ToArray()));

            return this;
        }

        public EntityElementBuilder Property(EntityPropertyElement property)
        {
            Childs(property);
            return this;
        }

        public EntityElementBuilder Relation(EntityRelationElement relation)
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