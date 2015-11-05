using System;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;

namespace NuClear.DataTest.Metamodel.Dsl
{
    public sealed class TestCaseMetadataElement : MetadataElement<TestCaseMetadataElement, TestCaseMetadataBuilder>
    {
        private IMetadataElementIdentity _identity;

        public TestCaseMetadataElement(ArrangeMetadataElement arrange, ActMetadataElement act)
            : base(new IMetadataFeature[0])
        {
            _identity = TestCaseMetadataIdentity.Instance.Id
                                                .WithRelative(arrange.Identity.Id)
                                                .WithRelative(act.Identity.Id)
                                                .AsIdentity();

            Arrange = arrange;
            Act = act;
        }

        public ArrangeMetadataElement Arrange { get; }

        public ActMetadataElement Act { get; }

        public override IMetadataElementIdentity Identity
            => _identity;

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotSupportedException();
        }
    }
}