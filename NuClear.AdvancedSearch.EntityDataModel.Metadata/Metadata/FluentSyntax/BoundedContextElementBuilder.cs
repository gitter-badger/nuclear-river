using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace
namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class BoundedContextElementBuilder : MetadataElementBuilder<BoundedContextElementBuilder, BoundedContextElement>
    {
        private const string ConceptualModelName = "ConceptualModel";
        private const string StoreModelName = "StoreModel";
        private const string MappingName = "StoreModel";

        private string _name;
        private StructuralModelElement _conceptualModel;
        private StructuralModelElement _storeModel;
        private ModelMappingElement _mapping;

        public BoundedContextElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public BoundedContextElementBuilder ConceptualModel(StructuralModelElement modelElement)
        {
            Childs(_conceptualModel = modelElement);
            return this;
        }

        public BoundedContextElementBuilder StoreModel(StructuralModelElement modelElement)
        {
            Childs(_storeModel = modelElement);
            return this;
        }

        public BoundedContextElementBuilder Mapping(ModelMappingElement conceptualToStoreMapping)
        {
            Childs(_mapping = conceptualToStoreMapping);
            return this;
        }

        protected override BoundedContextElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The context name was not specified.");
            }

            var contextId = IdBuilder.For<AdvancedSearchIdentity>(_name).AsIdentity();
            IMetadataElementIdentity conceptualModelId = null;
            IMetadataElementIdentity storeModelId = null;
            IMetadataElementIdentity mappingId = null;

            if (_conceptualModel != null)
            {
                conceptualModelId = contextId.Id.WithRelative(ConceptualModelName.AsRelativeUri()).AsIdentity();
                _conceptualModel.ActualizeId(conceptualModelId);
            }

            if (_storeModel != null)
            {
                storeModelId = contextId.Id.WithRelative(StoreModelName.AsRelativeUri()).AsIdentity();
                _storeModel.ActualizeId(storeModelId);
            }

            if (_mapping != null)
            {
                _mapping.ActualizeId(mappingId = contextId.Id.WithRelative(MappingName.AsRelativeUri()).AsIdentity());
                //UpdateMappings(_mapping);
            }

            return new BoundedContextElement(contextId, conceptualModelId, storeModelId, mappingId, Features);
        }

        private void UpdateMappings(ModelMappingElement mapping)
        {
            var conceptualEntities = _conceptualModel.GetFlattenEntities().ToDictionary(x => x.Identity.Id);
            var storeEntities = _storeModel.GetFlattenEntities().ToDictionary(x => x.Identity.Id);

            List<EntityMappingElement> processedMappings = new List<EntityMappingElement>();
            foreach (var mappingElement in _mapping.Mappings())
            {
                EntityElement conceptualEntity,
                              storeEntity;

                if (conceptualEntities.TryGetValue(mappingElement.ConceptualEntityIdentity.Id, out conceptualEntity)
                    && storeEntities.TryGetValue(mappingElement.StoreEntityIdentity.Id, out storeEntity))
                {
                    processedMappings.Add(new EntityMappingElement(mappingElement.Identity, conceptualEntity.Identity, storeEntity.Identity, mapping.Features));
                }
            }

            ((IMetadataElementUpdater)mapping).ReplaceChilds(processedMappings);
        }
    }
}