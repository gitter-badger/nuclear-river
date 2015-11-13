using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace NuClear.DataTest.Metamodel.Dsl
{
    public sealed class TestCaseMetadataIdentity : MetadataKindIdentityBase<TestCaseMetadataIdentity>
    {
        public override Uri Id { get; } 
            = Metadata.Id.For("TestCase");

        public override string Description 
            => "Test Case description";
    }
}