using System.Collections;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;
using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Processors;
using NuClear.Metamodeling.Provider;

using NUnit.Framework;

namespace NuClear.EntityDataModel.Tests
{
    [TestFixture]
    internal class BoundedContextMetadataTests : BaseMetadataFixture
    {
        private MetadataProvider _provider;

        public override IEnumerable Provider
        {
            get
            {
                yield return Case(BoundedContextElement.Config.Name("Context"))
                    .Returns("{'Identity':{'Id':'erm://metadata/AdvancedSearch/Context'},'Features':[]}")
                    .SetName("ShouldDeclareContext");
                yield return Case(BoundedContextElement
                                      .Config.Name("Context")
                                      .ConceptualModel(StructuralModelElement.Config.Elements(EntityElement.Config.Name("Entity"))),
                                  Metadata.Identity | Metadata.Elements)
                    .Returns("{'Identity':{'Id':'erm://metadata/AdvancedSearch/Context'},'Elements':[{'Identity':{'Id':'erm://metadata/AdvancedSearch/Context/ConceptualModel'},'Elements':[{'Identity':{'Id':'Entity'},'Elements':[]}]}]}")
                    .SetName("ShouldDeclareContextWithEntity");
            }
        }

        [TestFixtureSetUp]
        public void Setup()
        {
            var sources = new[] { new AdvancedSearchMetadataSource() };
            var processors = new IMetadataProcessor[] { };

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

        [TestCase("CustomerIntelligence", Metadata.Identity | Metadata.Elements | Metadata.Features, Explicit = true, Description = "Used to get dump of hierarchy (manual run)")]
        [TestCase("CustomerIntelligence/ConceptualModel", Metadata.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm", Metadata.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Id", Metadata.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Id'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Id", Metadata.Identity | Metadata.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Id'},'Features':[{'PropertyType':'Int64'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories", Metadata.Identity | Metadata.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Categories'},'Features':[{'Cardinality':'Many'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories/Category/Id", Metadata.Identity | Metadata.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Categories/Category/Id'},'Features':[{'PropertyType':'Int64'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories/Category/Name", Metadata.Identity | Metadata.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Categories/Category/Name'},'Features':[{'PropertyType':'String'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Categories/Category/CategoryGroup", Metadata.Identity | Metadata.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Categories/Category/CategoryGroup'},'Features':[{'PropertyType':'Byte'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Client", Metadata.Identity | Metadata.Features, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Client'},'Features':[{'Cardinality':'OptionalOne'}]}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Client/Client/Id", Metadata.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Client/Client/Id'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Client/Client/CategoryGroup", Metadata.Identity, Result = "{'Identity':{'Id':'erm://metadata/AdvancedSearch/CustomerIntelligence/ConceptualModel/Firm/Client/Client/CategoryGroup'}}")]
        [TestCase("CustomerIntelligence/ConceptualModel/Firm/Client/Client/Contacts/Contact/Role", Metadata.Features, Result = "{'Features':[{'Name':'ContactRole','UnderlyingType':'Int32','Members':{'Employee':200000,'InfluenceDecisions':200001,'MakingDecisions':200002},'PropertyType':'Enum'}]}")]
        public string ShouldReturnMetadata(string path, Metadata properties)
        {
            IMetadataElement element;

            var id = IdBuilder.For<AdvancedSearchIdentity>(path.Split('/'));
            Assert.That(_provider.TryGetMetadata(id, out element), Is.True);
            Assert.That(element, Is.Not.Null);

            return Serialize(element, properties);
        }
    }
}
