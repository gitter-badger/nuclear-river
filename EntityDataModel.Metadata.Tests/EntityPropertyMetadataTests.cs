using System;
using System.Collections;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Tests
{
    [TestFixture]
    internal class EntityPropertyMetadataTests : BaseMetadataFixture<EntityPropertyElement, EntityPropertyElementBuilder>
    {
        public override IEnumerable TestCases
        {
            get
            {
                yield return Case(EntityPropertyElement.Config)
                    .Throws(typeof(InvalidOperationException))
                    .SetName("ShouldFailIfNoName");
                yield return Case(EntityPropertyElement.Config.Name("Property"))
                    .Throws(typeof(InvalidOperationException))
                    .SetName("ShouldFailIfNoType");
                yield return Case(EntityPropertyElement.Config.Name("Property").OfType(EntityPropertyType.String), MetadataKind.Identity | MetadataKind.Features)
                    .Returns("{'Identity':{'Id':'Property'},'Features':[{'PropertyType':'String'}]}")
                    .SetName("ShouldDeclareProperty");
                yield return Case(EntityPropertyElement.Config.Name("Property").OfType(EntityPropertyType.String).Nullable(), MetadataKind.Features)
                    .Returns("{'Features':[{'IsNullable':true},{'PropertyType':'String'}]}")
                    .SetName("ShouldDeclareNullableProperty");
                yield return Case(EntityPropertyElement.Config.Name("Property").UsingEnum("Gender").WithMember("Female", 1).WithMember("Male", 2), MetadataKind.Features)
                    .Returns("{'Features':[{'Name':'Gender','UnderlyingType':'Int32','Members':{'Female':1,'Male':2},'PropertyType':'Enum'}]}")
                    .SetName("ShouldDeclareEnumProperty");
            }
        }
    }
}
