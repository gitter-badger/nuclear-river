using System.Diagnostics;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Provider.Sources;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Tests
{
    [TestFixture]
    internal class CustomerIntelligenceMetadataTests
    {
        private MetadataProvider _provider;

        [TestFixtureSetUp]
        public void Setup()
        {
            var sources = new IMetadataSource[] { new AdvancedSearchMetadataSource() };
            var processors = new IMetadataProcessor[] {};

            _provider = new MetadataProvider(sources, processors);
        }

        [Test]
        public void ShouldReturnMetadataByIdentity()
        {
            MetadataSet metadataSet;
            
            Assert.That(_provider.TryGetMetadata<AdvancedSearchIdentity>(out metadataSet), Is.True);
            Assert.That(metadataSet, Is.Not.Null);
        }

        [TestCase("CustomerIntelligence")]
        public void ShouldReturnMetadataByContextId(string name)
        {
            BoundedContextElement contextElement;

            var id = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<AdvancedSearchIdentity>(name);
            Assert.That(_provider.TryGetMetadata(id, out contextElement), Is.True);
            Assert.That(contextElement, Is.Not.Null);
        }

        [TestCase("CustomerIntelligence", MetadataKind.Identity | MetadataKind.Elements | MetadataKind.Features, Explicit = true, Description = "Used to get dump of hierarchy (manual run)")]
        [TestCase("CustomerIntelligence/ConceptualModel/CategoryGroup", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/CategoryGroup'},'Features':[{'EntitySetName':'CategoryGroups'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Project", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Project'},'Features':[{'EntitySetName':'Projects'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Project/Categories", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Project/Categories'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/ProjectCategory'},'Features':[{'EntitySetName':'ProjectCategories'}]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/ProjectCategory", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/ProjectCategory'},'Features':[{'EntitySetName':'ProjectCategories'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Territory", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Territory'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm'},'Features':[{'EntitySetName':'Firms'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Balances", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Balances'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmBalance'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Categories'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategory'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/CategoryGroup", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/CategoryGroup'},'Features':[{'Cardinality':'One','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/CategoryGroup'},'Features':[{'EntitySetName':'CategoryGroups'}]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Client", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Client'},'Features':[{'Cardinality':'OptionalOne','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/OwnerId", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/OwnerId'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Territories", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Territories'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmTerritory'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmBalance", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmBalance'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategory'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client/CategoryGroup", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client/CategoryGroup'},'Features':[{'Cardinality':'One','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/CategoryGroup'},'Features':[{'EntitySetName':'CategoryGroups'}]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client/Contacts", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client/Contacts'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/ClientContact'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/ClientContact", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/ClientContact'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmTerritory", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmTerritory'},'Features':[]}")]
        public string ShouldReturnMetadata(string path, MetadataKind properties)
        {
            IMetadataElement element;

            Assert.That(TryGetMetadata(path, out element), Is.True);
            Assert.That(element, Is.Not.Null);

            var ignoredFeatures = new[] { typeof(EntityIdentityFeature), typeof(ElementMappingFeature), typeof(EntityRelationContainmentFeature) };
            element.Dump(properties, ignoredFeatures);

            Debug.WriteLine(element.Serialize(properties, ignoredFeatures));

            return element.Serialize(properties, ignoredFeatures);
        }

        [TestCase("CustomerIntelligence")]
        [TestCase("CustomerIntelligence/ConceptualModel")]
        [TestCase("CustomerIntelligence/ConceptualModel/CategoryGroup")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client")]
        [TestCase("CustomerIntelligence/ConceptualModel/ClientContact")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmBalance")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory")]
        [TestCase("CustomerIntelligence/ConceptualModel/Project")]
        [TestCase("CustomerIntelligence/ConceptualModel/ProjectCategory")]
        [TestCase("CustomerIntelligence/ConceptualModel/Territory")]
        [TestCase("CustomerIntelligence/StoreModel")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.CategoryGroup")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Client")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.ClientContact")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmView")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmBalance")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Project")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.ProjectCategory")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Territory")]
        public void ShouldBeDeclared(string path)
        {
            IMetadataElement element;

            Assert.That(TryGetMetadata(path, out element), Is.True);
            Assert.That(element, Is.Not.Null);
        }

        private bool TryGetMetadata(string path, out IMetadataElement element)
        {
            var id = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<AdvancedSearchIdentity>(path);
            return _provider.TryGetMetadata(id, out element);
        }
    }
}
