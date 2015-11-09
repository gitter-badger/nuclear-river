using System.Linq;

using NuClear.Metamodeling.Elements;

namespace NuClear.DataTest.Metamodel.Dsl
{
    public sealed class ArrangeMetadataElementBuilder : MetadataElementBuilder<ArrangeMetadataElementBuilder, ArrangeMetadataElement>
    {
        private string _name;

        public ArrangeMetadataElementBuilder Name(string name)
        {
            _name = name;
            return this;
        }

        protected override ArrangeMetadataElement Create()
        {
            return new ArrangeMetadataElement(_name, Features);
        }

        public ArrangeMetadataElementBuilder IncludeSharedDictionary(ArrangeMetadataElement dictionaryArrange)
            => WithFeatures(dictionaryArrange.Features.ToArray());
    }
}