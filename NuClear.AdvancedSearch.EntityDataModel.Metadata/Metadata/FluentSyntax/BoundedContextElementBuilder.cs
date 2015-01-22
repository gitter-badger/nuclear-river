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

        private string _name;
        private StructuralModelElement _conceptualModel;
        private StructuralModelElement _storeModel;
        private readonly IDictionary<string,string> _entityMap = new Dictionary<string, string>();

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
            IMetadataElementIdentity conceptualModelId = null;
            IMetadataElementIdentity storeModelId = null;
            IMetadataElementIdentity mappingId = null;

            if (_conceptualModel != null)
            {
                conceptualModelId = contextId.Id.WithRelative(ConceptualModelName.AsUri()).AsIdentity();
                _conceptualModel.ActualizeId(conceptualModelId);
            }

            if (_storeModel != null)
            {
                storeModelId = contextId.Id.WithRelative(StoreModelName.AsUri()).AsIdentity();
                _storeModel.ActualizeId(storeModelId);
            }

            ProcessMappings();

            return new BoundedContextElement(contextId, conceptualModelId, storeModelId, Features);
        }

        private void ProcessMappings()
        {
            if (_conceptualModel == null || _storeModel == null)
            {
                return;
            }

            var entities = _conceptualModel.GetFlattenEntities().ToDictionary(x => x.Identity.Id.ToString());

            foreach (var map in _entityMap)
            {
                EntityElement entityElement;
                if (entities.TryGetValue(map.Key, out entityElement))
                {
                    ((IMetadataElementUpdater)entityElement).AddFeature(new ElementMappingFeature(map.Value.AsUri().AsIdentity()));
                }
            }
        }
    }
}