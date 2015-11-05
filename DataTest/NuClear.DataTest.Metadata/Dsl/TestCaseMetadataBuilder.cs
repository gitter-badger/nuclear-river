using System;

using NuClear.Metamodeling.Elements;

namespace NuClear.DataTest.Metamodel.Dsl
{
    public sealed class TestCaseMetadataBuilder : MetadataElementBuilder<TestCaseMetadataBuilder, TestCaseMetadataElement>
    {
        protected override TestCaseMetadataElement Create()
        {
            throw new NotSupportedException("by design");
        }
    }
}