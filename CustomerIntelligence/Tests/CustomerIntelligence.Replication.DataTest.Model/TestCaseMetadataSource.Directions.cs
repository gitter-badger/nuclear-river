using NuClear.CustomerIntelligence.Replication.Tests.BulkLoading;
using NuClear.DataTest.Metamodel.Dsl;

namespace CustomerIntelligence.Replication.DataTest.Model
{
    public sealed partial class TestCaseMetadataSource
    {
        private static readonly ActMetadataElement ErmToFact =
            ActMetadataElement.Config
                              .Source("Erm")
                              .Target("Facts")
                              .Action<BulkReplication<BulkLoadingOfFacts>>();

        private static readonly ActMetadataElement FactToCi =
            ActMetadataElement.Config
                              .Source("Facts")
                              .Target("CustomerIntelligence")
                              .Action<BulkReplication<BulkLoadingOfCustomerIntelligence>>();

        private static readonly ActMetadataElement BitToStatistics =
            ActMetadataElement.Config
                              .Source("Bit")
                              .Target("Statistics")
                              .Require("CustomerIntelligence")
                              .Action<PrefillStatisticsContext>()
                              .Action<BulkReplication<BulkLoadingOfStatistics>>();
    }
}
