using System.Collections;

using NuClear.AdvancedSearch.EntityDataModel.Metadata;

using NUnit.Framework;

namespace NuClear.EntityDataModel.Tests
{
    [TestFixture]
    internal class EntityPropertyMetadataTests : BaseMetadataFixture
    {
        public override IEnumerable Provider
        {
            get
            {
                yield return Case(
                    EntityPropertyElement.Config.Name("Property"),
                    "{'Identity':{'Id':'Property'},'Features':[]}"
                    ).SetName("ShouldDeclareProperty");
                yield return Case(
                    EntityPropertyElement.Config.Name("Property").OfType(EntityPropertyType.Int64), 
                    "{'Identity':{'Id':'Property'},'Features':[{'PropertyType':'Int64'}]}"
                    ).SetName("ShouldDeclareTypedProperty");
                yield return Case(
                    EntityPropertyElement.Config.Name("Property").NotNull(), 
                    "{'Identity':{'Id':'Property'},'Features':[{'IsNullable':false}]}"
                    ).SetName("ShouldDeclareNonNullableProperty");
                yield return Case(
                    EntityPropertyElement.Config.Name("Property")
                        .UsingEnum("Gender")
                        .WithMember("Female", 1)
                        .WithMember("Male", 2),
                    "{'Identity':{'Id':'Property'},'Features':[{'Name':'Gender','UnderlyingType':'Int32','Members':{'Female':1,'Male':2},'PropertyType':'Enum'}]}"
                    ).SetName("ShouldDeclareEnumProperty");
            }
        }
    }
}
