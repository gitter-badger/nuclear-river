using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class EntityMappingElementBuilder : MetadataElementBuilder<EntityMappingElementBuilder, EntityMappingElement>
    {
        private string _conceptualEntityName;
        private string _storeEntityName;

        public EntityMappingElementBuilder Map(string conceptualEntityName, string storeEntityName)
        {
            _conceptualEntityName = conceptualEntityName;
            _storeEntityName = storeEntityName;
            return this;
        }

        public EntityMappingElementBuilder Map(PropertyMappingElement mappingElement)
        {
            return this;
        }

        protected override EntityMappingElement Create()
        {
            var identity = IdBuilder.For<AdvancedSearchIdentity>(_conceptualEntityName, _storeEntityName).AsIdentity();
            var sourceEntityIdentity = IdBuilder.For<AdvancedSearchIdentity>(_conceptualEntityName).AsIdentity();
            var targetEntityIdentity = IdBuilder.For<AdvancedSearchIdentity>(_storeEntityName).AsIdentity();

            return new EntityMappingElement(identity, sourceEntityIdentity, targetEntityIdentity, Features);
        }
    }
}