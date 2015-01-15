using System.Collections;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;

using NUnit.Framework;

namespace NuClear.EntityDataModel.Tests
{
    [TestFixture]
    internal class EntityRelationMetadataTests : BaseMetadataFixture
    {
        public override IEnumerable Provider
        {
            get
            {
                yield return
                    Case(EntityRelationElement.Config.Name("Relation"))
                        .Returns("{'Identity':{'Id':'Relation'},'Features':[]}")
                        .SetName("ShouldDeclareRelation");
                yield return
                    Case(EntityRelationElement.Config.Name("Relation").AsOneOptionally())
                        .Returns("{'Identity':{'Id':'Relation'},'Features':[{'Cardinality':'OptionalOne'}]}")
                        .SetName("ShouldDeclareRelationAsOneOrZero");
                yield return
                    Case(EntityRelationElement.Config.Name("Relation").AsOne())
                        .Returns("{'Identity':{'Id':'Relation'},'Features':[{'Cardinality':'One'}]}")
                        .SetName("ShouldDeclareRelationAsOne");
                yield return
                    Case(EntityRelationElement.Config.Name("Relation").AsMany())
                        .Returns("{'Identity':{'Id':'Relation'},'Features':[{'Cardinality':'Many'}]}")
                        .SetName("ShouldDeclareRelationAsMany");
            }
        }
    }
}
