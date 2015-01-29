using System.Collections;

using NuClear.Metamodeling.Elements;

using NUnit.Framework;

namespace NuClear.AdvancedSearch.EntityDataModel.Metadata.Tests
{
    internal abstract class BaseMetadataFixture<TMetadataElement, TBuilder>
        where TMetadataElement : MetadataElement
        where TBuilder : MetadataElementBuilder<TBuilder, TMetadataElement>, new()
    {
        public abstract IEnumerable TestCases { get; }

        [Test, TestCaseSource("TestCases")]
        public string CheckSerialization(TBuilder element, MetadataKind properties)
        {
            return SerializeAndDump(element, properties);
        }

        protected static TestCaseData Case(TBuilder config)
        {
            return Case(config, MetadataKind.Identity | MetadataKind.Features);
        }

        protected static TestCaseData Case(TBuilder config, MetadataKind properties)
        {
            return new TestCaseData(config, properties);
        }

        private static string SerializeAndDump(TMetadataElement element, MetadataKind metadata)
        {
            element.Dump(metadata);

            return element.Serialize(metadata);
        }
   }
}