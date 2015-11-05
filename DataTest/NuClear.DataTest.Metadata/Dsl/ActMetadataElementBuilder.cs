using System.Linq;

using NuClear.Metamodeling.Elements;

namespace NuClear.DataTest.Metamodel.Dsl
{
    public sealed class ActMetadataElementBuilder : MetadataElementBuilder<ActMetadataElementBuilder, ActMetadataElement>
    {
        public ActMetadataElementBuilder Source(string context)
            => WithFeatures(new ActMetadataElement.SourceContextFeature(context), new ActMetadataElement.RequiredContextFeature(context));

        public ActMetadataElementBuilder Require(string context)
            => WithFeatures(new ActMetadataElement.RequiredContextFeature(context));

        public ActMetadataElementBuilder Target(string context)
            => WithFeatures(new ActMetadataElement.TargetContextFeature(context));

        public ActMetadataElementBuilder Action<T>()
            where T : ITestAction
            => WithFeatures(new ActMetadataElement.ActionFeature(Features.OfType<ActMetadataElement.ActionFeature>().Count(), typeof(T)));

        protected override ActMetadataElement Create()
        {
            return new ActMetadataElement(Features);
        }
    }
}