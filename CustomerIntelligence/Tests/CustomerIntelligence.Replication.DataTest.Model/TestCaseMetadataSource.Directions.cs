using NuClear.DataTest.Metamodel.Dsl;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    public sealed partial class TestCaseMetadataSource
    {
        private static readonly ActMetadataElement ErmToFact =
            ActMetadataElement.Config
                              .Source(ContextName.Erm)
                              .Target(ContextName.Facts)
                              .Action<BulkReplicationAdapter<Facts>>();

        private static readonly ActMetadataElement FactToCi =
            ActMetadataElement.Config
                              .Source(ContextName.Facts)
                              .Target(ContextName.CustomerIntelligence)
                              .Action<BulkReplicationAdapter<CustomerIntelligence>>();

        private static readonly ActMetadataElement BitToStatistics =
            ActMetadataElement.Config
                              .Source(ContextName.Bit)
                              .Target(ContextName.Statistics)
                              .Require(ContextName.CustomerIntelligence)
                              .Action<PrefillStatisticsContext>()
                              .Action<BulkReplicationAdapter<Statistics>>();
    }
}
