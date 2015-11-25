using NuClear.DataTest.Metamodel.Dsl;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    public sealed partial class TestCaseMetadataSource
    {
        private static readonly ActMetadataElement ErmToFact =
            ActMetadataElement.Config
                              .Source("Erm")
                              .Target("Facts")
                              .Action<BulkReplicationAdapter<Facts>>();

        private static readonly ActMetadataElement FactToCi =
            ActMetadataElement.Config
                              .Source("Facts")
                              .Target("CustomerIntelligence")
                              .Action<BulkReplicationAdapter<CustomerIntelligence>>();

        private static readonly ActMetadataElement BitToStatistics =
            ActMetadataElement.Config
                              .Source("Bit")
                              .Target("Statistics")
                              .Require("CustomerIntelligence")
                              .Action<PrefillStatisticsContext>()
                              .Action<BulkReplicationAdapter<Statistics>>();
    }
}
