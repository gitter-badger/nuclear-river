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
        private string _name;
        private string _entitySetName;
        private readonly List<string> _keyNames = new List<string>();

        public EntityElementBuilder Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name should be meaningful.", "name");
            }
            _name = name;
            return this;
        }

        public EntityElementBuilder EntitySetName(string entitySetName)
        {
            if (string.IsNullOrWhiteSpace(entitySetName))
            {
                throw new ArgumentException("The entity set name should be meaningful.", "entitySetName");
            }
            _entitySetName = entitySetName;
            return this;
        }

        public EntityElementBuilder IdentifyBy(params string[] propertyNames)
        {
            if (propertyNames.Length == 0)
            {
                throw new ArgumentException("At least one name should be specified.", "propertyNames");
            }

            _keyNames.AddRange(propertyNames);
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
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The entity name was not specified.");
            }

            ProcessKeys();
            ProcessCollectionName();

            return new EntityElement(_name.AsRelativeUri().AsIdentity(), Features);
        }

        private void ProcessKeys()
        {
            if (_keyNames.Count == 0)
            {
                return;
            }

            var properties = ChildElements.OfType<EntityPropertyElement>().ToDictionary(x => x.Identity.Id);

            Func<string, EntityPropertyElement> resolve = propertyName =>
            {
                EntityPropertyElement property;

                var propertyId = propertyName.AsRelativeUri();
                if (!properties.TryGetValue(propertyId, out property))
                {
                    throw new InvalidOperationException(string.Format("The property with name: '{0}' was not declared.", propertyName));
                }
                return property;
            };

            AddFeatures(new EntityIdentityFeature(_keyNames.Distinct().Select(propertyName => resolve(propertyName)).ToArray()));
        }

        private void ProcessCollectionName()
        {
            if (string.IsNullOrEmpty(_entitySetName))
            {
                return;
            }
            AddFeatures(new EntitySetFeature(_entitySetName));
        }
    }
}