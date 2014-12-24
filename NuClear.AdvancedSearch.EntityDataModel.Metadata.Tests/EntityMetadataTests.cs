using System.Collections;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;

using NUnit.Framework;

namespace NuClear.EntityDataModel.Tests
{
    [TestFixture]
    internal class EntityMetadataTests : BaseMetadataFixture
    {
        public override IEnumerable Provider
        {
            get
            {
                yield return Case(
                    EntityElement.Config.Name("Entity"),
                    "{'Identity':{'Id':'Entity'},'Features':[]}"
                    ).SetName("ShouldDeclareEntity");
                yield return Case(
                    EntityElement.Config.Name("Entity").CollectionName("Entities"),
                    "{'Identity':{'Id':'Entity'},'Features':[{'CollectionName':'Entities'}]}"
                    ).SetName("ShouldDeclareEntityAsCollection");
                yield return Case(
                    EntityElement.Config.Name("Entity").Property(EntityPropertyElement.Config.Name("Property")),
                    "{'Identity':{'Id':'Entity'},'Elements':[{'Identity':{'Id':'Property'},'Elements':[]}]}", Metadata.Identity | Metadata.Elements
                    ).SetName("ShouldDeclareEntityWithProperty");
                yield return Case(
                    EntityElement.Config.Name("Entity").IdentifyBy("Property").Property(EntityPropertyElement.Config.Name("Property")),
                    "{'Identity':{'Id':'Entity'},'Features':[{'IdentifyingProperties':[{'Identity':{'Id':'Property'},'Features':[]}]}]}"
                    ).SetName("ShouldDeclareEntityWithKey");
                yield return Case(
                    EntityElement.Config.Name("Entity").Relation(EntityRelationElement.Config.Name("Relation")),
                    "{'Identity':{'Id':'Entity'},'Elements':[{'Identity':{'Id':'Relation'},'Elements':[]}]}", Metadata.Identity | Metadata.Elements
                    ).SetName("ShouldDeclareEntityWithRelation");
            }
        }
    }
}
