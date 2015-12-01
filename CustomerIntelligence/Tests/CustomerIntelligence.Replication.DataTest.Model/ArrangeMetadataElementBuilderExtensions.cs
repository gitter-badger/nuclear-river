using NuClear.DataTest.Metamodel.Dsl;

namespace NuClear.CustomerIntelligence.Replication.StateInitialization.Tests
{
    public static class ArrangeMetadataElementBuilderExtensions
    {
        public static ArrangeMetadataElementBuilder CustomerIntelligence(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature(ContextName.CustomerIntelligence, data));

        public static ArrangeMetadataElementBuilder Fact(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature(ContextName.Facts, data));

        public static ArrangeMetadataElementBuilder Erm(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature(ContextName.Erm, data));

        public static ArrangeMetadataElementBuilder Bit(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature(ContextName.Bit, data));

        public static ArrangeMetadataElementBuilder Statistics(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature(ContextName.Statistics, data));
    }
}
