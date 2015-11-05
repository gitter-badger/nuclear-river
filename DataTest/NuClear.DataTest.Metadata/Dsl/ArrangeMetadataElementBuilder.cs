using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;

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
    }
}