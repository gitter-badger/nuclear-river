using NuClear.DataTest.Metamodel.Dsl;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    public static class ArrangeMetadataElementBuilderExtensions
    {
        public static ArrangeMetadataElementBuilder CustomerIntelligence(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature("CustomerIntelligence", data));

        public static ArrangeMetadataElementBuilder Fact(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature("Facts", data));

        public static ArrangeMetadataElementBuilder Erm(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature("Erm", data));

        public static ArrangeMetadataElementBuilder Bit(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature("Bit", data));

        public static ArrangeMetadataElementBuilder Statistics(this ArrangeMetadataElementBuilder builder, params object[] data)
            => builder.WithFeatures(new ArrangeMetadataElement.ContextStateFeature("Statistics", data));
    }
}
