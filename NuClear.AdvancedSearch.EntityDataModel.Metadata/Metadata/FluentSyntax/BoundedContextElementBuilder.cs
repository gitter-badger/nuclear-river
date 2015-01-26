using System;
using System.Collections.Generic;
using System.Linq;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;

// ReSharper disable once CheckNamespace

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata
{
    public sealed class BoundedContextElementBuilder : MetadataElementBuilder<BoundedContextElementBuilder, BoundedContextElement>
    {
        private const string ConceptualModelName = "ConceptualModel";
        private const string StoreModelName = "StoreModel";

        private readonly IDictionary<string, string> _entityMap = new Dictionary<string, string>();
        private string _name;
        private StructuralModelElementBuilder _conceptualModel;
        private StructuralModelElementBuilder _storeModel;

        public BoundedContextElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        public BoundedContextElementBuilder ConceptualModel(StructuralModelElementBuilder modelElement)
        {
            _conceptualModel = modelElement.Name(ConceptualModelName);
            return this;
        }

        public BoundedContextElementBuilder StoreModel(StructuralModelElementBuilder modelElement)
        {
            _storeModel = modelElement.Name(StoreModelName);
            return this;
        }

        public BoundedContextElementBuilder Map(string conceptualEntityName, string storeEntityName)
        {
            _entityMap.Add(conceptualEntityName, storeEntityName);
            return this;
        }

        protected override BoundedContextElement Create()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("The context name was not specified.");
            }

            var contextId = IdBuilder.For<AdvancedSearchIdentity>(_name).AsIdentity();

            StructuralModelElement conceptualModel = null;
            StructuralModelElement storeModel = null;

            if (_conceptualModel != null)
            {
                conceptualModel = _conceptualModel;
                Childs(conceptualModel);
            }

            if (_storeModel != null)
            {
                storeModel = _storeModel;
                Childs(storeModel);
            }

            ProcessMappings(conceptualModel, storeModel);

            return new BoundedContextElement(contextId, conceptualModel, storeModel, Features);
        }

        private void ProcessMappings(StructuralModelElement conceptualModel, StructuralModelElement storeModel)
        {
            if (conceptualModel == null || storeModel == null)
            {
                return;
            }

            var conceptualEntities = conceptualModel.Entities.ToDictionary(x => x.Identity.Id);
            var storeEntities = storeModel.Entities.ToDictionary(x => x.Identity.Id);

            foreach (var map in _entityMap)
            {
                EntityElement conceptualEntity;
                EntityElement storeEntity;
                if (!conceptualEntities.TryGetValue(map.Key.AsUri(), out conceptualEntity) || !storeEntities.TryGetValue(map.Value.AsUri(), out storeEntity))
                {
                    throw new InvalidOperationException("The entity mapping cannot be resolved.");
                }

                ((IMetadataElementUpdater)conceptualEntity).AddFeature(new ElementMappingFeature(storeEntity));
            }
        }
    }
}