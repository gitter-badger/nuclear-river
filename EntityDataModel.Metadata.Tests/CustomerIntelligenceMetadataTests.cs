using System.Diagnostics;

using NuClear.AdvancedSearch.EntityDataModel.Metadata.Features;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
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

        [Test]
        public void ShouldReturnMetadataByContextId()
        {
            BoundedContextElement contextElement;

            var id = IdBuilder.For<AdvancedSearchIdentity>("CustomerIntelligence");
            Assert.That(_provider.TryGetMetadata(id, out contextElement), Is.True);
            Assert.That(contextElement, Is.Not.Null);
        }

        [TestCase("CustomerIntelligence", MetadataKind.Identity | MetadataKind.Elements | MetadataKind.Features, Explicit = true, Description = "Used to get dump of hierarchy (manual run)")]
        [TestCase("CustomerIntelligence/ConceptualModel", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/OrganizationUnit", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/OrganizationUnit'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Territory", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Territory'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory1", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategory1'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory2", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategory2'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/FirmCategory3", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategory3'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Category", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Category'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Contact", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Contact'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Account", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Account'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories1", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Categories1'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategory1'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories2", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Categories2'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategory2'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories3", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Categories3'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/FirmCategory3'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Client", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Client'},'Features':[{'Cardinality':'OptionalOne','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client/Accounts", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client/Accounts'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Account'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Client/Contacts", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Client/Contacts'},'Features':[{'Cardinality':'Many','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Contact'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/StoreModel", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.OrganizationUnit", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.OrganizationUnit'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Territory", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Territory'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Category", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Category'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Firm", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Firm'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory1", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory1'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory2", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory2'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory3", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory3'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Client", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Client'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Contact", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Contact'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Account", MetadataKind.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Account'}}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Account/ClientId", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Account/ClientId'},'Features':[{'Cardinality':'One','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Client'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory1/FirmId", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory1/FirmId'},'Features':[{'Cardinality':'One','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Firm'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory2/FirmId", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory2/FirmId'},'Features':[{'Cardinality':'One','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Firm'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory3/FirmId", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.FirmCategory3/FirmId'},'Features':[{'Cardinality':'One','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Firm'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Contact/ClientId", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Contact/ClientId'},'Features':[{'Cardinality':'One','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Client'},'Features':[]}}]}")]
        [TestCase("CustomerIntelligence/StoreModel/CustomerIntelligence.Firm/ClientId", MetadataKind.Identity | MetadataKind.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Firm/ClientId'},'Features':[{'Cardinality':'OptionalOne','Target':{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/StoreModel/CustomerIntelligence.Client'},'Features':[]}}]}")]
        public string ShouldReturnMetadata(string path, MetadataKind properties)
        {
            IMetadataElement element;

            var id = IdBuilder.For<AdvancedSearchIdentity>(path);
            Assert.That(_provider.TryGetMetadata(id, out element), Is.True);
            Assert.That(element, Is.Not.Null);

            var ignoredFeatures = new[] { typeof(EntityIdentityFeature), typeof(ElementMappingFeature) };
            element.Dump(properties, ignoredFeatures);

            Debug.WriteLine(element.Serialize(properties, ignoredFeatures));

            return element.Serialize(properties, ignoredFeatures);
        }
    }
}
