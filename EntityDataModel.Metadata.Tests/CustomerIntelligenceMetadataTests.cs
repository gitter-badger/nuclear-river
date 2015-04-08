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

        [TestCase("BusinessDirectory")]
        [TestCase("CustomerIntelligence")]
        public void ShouldReturnMetadataByContextId(string name)
        {
            BoundedContextElement contextElement;

            var id = Metamodeling.Elements.Identities.Builder.Metadata.Id.For<AdvancedSearchIdentity>(name);
            Assert.That(_provider.TryGetMetadata(id, out contextElement), Is.True);
            Assert.That(contextElement, Is.Not.Null);
        }

        [TestCase("BusinessDirectory", MetadataKind.Identity | MetadataKind.Elements | MetadataKind.Features, Explicit = true, Description = "Used to get dump of hierarchy (manual run)")]
        [TestCase("BusinessDirectory/ConceptualModel/Category", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/BusinessDirectory/ConceptualModel/Category'},'Features':[{'EntitySetName':'Categories'}]}")]
        [TestCase("BusinessDirectory/ConceptualModel/CategoryGroup", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/BusinessDirectory/ConceptualModel/CategoryGroup'},'Features':[{'EntitySetName':'CategoryGroups'}]}")]
        [TestCase("BusinessDirectory/ConceptualModel/OrganizationUnit", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/BusinessDirectory/ConceptualModel/OrganizationUnit'},'Features':[{'EntitySetName':'OrganizationUnits'}]}")]
        [TestCase("BusinessDirectory/ConceptualModel/Territory", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/BusinessDirectory/ConceptualModel/Territory'},'Features':[{'EntitySetName':'Territories'}]}")]
        [TestCase("BusinessDirectory/ConceptualModel/Territory/OrganizationUnit", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/BusinessDirectory/ConceptualModel/Territory/OrganizationUnit'},'Features':[{'Cardinality':'One','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/BusinessDirectory/ConceptualModel/OrganizationUnit'},'Features':[{'EntitySetName':'OrganizationUnits'}]}}]}")]
        [TestCase("CustomerIntelligence", MetadataKind.Identity | MetadataKind.Elements | MetadataKind.Features, Explicit = true, Description = "Used to get dump of hierarchy (manual run)")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Contact", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Contact'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm'},'Features':[{'EntitySetName':'Firms'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmAccount", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmAccount'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategory'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategoryGroup", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategoryGroup'},'Features':[]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Accounts", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Accounts'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmAccount'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Client", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Client'},'Features':[{'Cardinality':'OptionalOne','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Categories'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategory'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/CategoryGroupId", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/CategoryGroupId'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/CategoryGroups", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/CategoryGroups'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategoryGroup'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/OrganizationUnitId", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/OrganizationUnitId'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/TerritoryId", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/TerritoryId'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client/Contacts", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client/Contacts'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Contact'},'Features':[]}}]}")]
        public string ShouldReturnMetadata(string path, MetadataKind properties)
        {
            IMetadataElement element;

            Assert.That(TryGetMetadata(path, out element), Is.True);
            Assert.That(element, Is.Not.Null);

            var ignoredFeatures = new[] { typeof(EntityIdentityFeature), typeof(ElementMappingFeature) };
            element.Dump(properties, ignoredFeatures);

            Debug.WriteLine(element.Serialize(properties, ignoredFeatures));

            return element.Serialize(properties, ignoredFeatures);
        }

        [TestCase("BusinessDirectory")]
        [TestCase("BusinessDirectory/ConceptualModel")]
        [TestCase("BusinessDirectory/ConceptualModel/Category")]
        [TestCase("BusinessDirectory/ConceptualModel/CategoryGroup")]
        [TestCase("BusinessDirectory/ConceptualModel/OrganizationUnit")]
        [TestCase("BusinessDirectory/ConceptualModel/Territory")]
        [TestCase("BusinessDirectory/StoreModel")]
        [TestCase("BusinessDirectory/StoreModel/BusinessDirectory.Category")]
        [TestCase("BusinessDirectory/StoreModel/BusinessDirectory.CategoryGroup")]
        [TestCase("BusinessDirectory/StoreModel/BusinessDirectory.OrganizationUnit")]
        [TestCase("BusinessDirectory/StoreModel/BusinessDirectory.Territory")]
        [TestCase("CustomerIntelligence")]
        [TestCase("CustomerIntelligence/ConceptualModel")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client")]
        [TestCase("CustomerIntelligence/ConceptualModel/Contact")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmAccount")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategoryGroup")]
        [TestCase("CustomerIntelligence/StoreModel")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Client")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Contact")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Firm")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmAccount")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategories")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategoryGroups")]
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
