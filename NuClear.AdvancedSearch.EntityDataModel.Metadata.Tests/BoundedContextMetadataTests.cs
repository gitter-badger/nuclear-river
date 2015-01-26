using System;
using System.Collections;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;

using NUnit.Framework;

namespace NuClear.EntityDataModel.Tests
{
    [TestFixture]
    internal class BoundedContextMetadataTests : BaseMetadataFixture<BoundedContextElement, BoundedContextElementBuilder>
    {
        public override IEnumerable TestCases
        {
            get
            {
                yield return Case(BoundedContextElement.Config)
                    .Throws(typeof(InvalidOperationException))
                    .SetName("ShouldFailIfNoName");
                yield return Case(BoundedContextElement.Config.Name("Context"), MetadataKind.Identity)
                    .Returns("{'Identity':{'Id':'erm://metadata/AdvancedSearch/Context'}}")
                    .SetName("ShouldDeclareContext");
                yield return Case(BoundedContextElement.Config.Name("Context"), MetadataKind.Elements | MetadataKind.Features)
                    .Returns("{'Features':[],'Elements':[]}")
                    .SetName("ShouldDeclareEmptyContext");
                yield return Case(BoundedContextElement.Config.Name("Context").ConceptualModel(StructuralModelElement.Config), MetadataKind.Identity | MetadataKind.Elements)
                    .Returns("{'Identity':{'Id':'erm://metadata/AdvancedSearch/Context'},'Elements':[{'Identity':{'Id':'ConceptualModel'},'Elements':[]}]}")
                    .SetName("ShouldDeclareContextWithConceptualModel");
                yield return Case(BoundedContextElement.Config.Name("Context").StoreModel(StructuralModelElement.Config), MetadataKind.Identity | MetadataKind.Elements)
                    .Returns("{'Identity':{'Id':'erm://metadata/AdvancedSearch/Context'},'Elements':[{'Identity':{'Id':'StoreModel'},'Elements':[]}]}")
                    .SetName("ShouldDeclareContextWithStoreModel");
            }
        }
    }
}
