using NUnit.Framework;

namespace NuClear.AdvancedSearch.Replication.Bulk.Tests
{
    [TestFixture]
    public sealed class BulkReplication
    {
        [Ignore("Use for bulk replication")]
        [Test]
        public void FactReplicationTest()
        {
            Program.Main(new[] { "-fact" });
        }

        [Ignore("Use for bulk replication")]
        [Test]
        public void CiReplicationTest()
        {
            Program.Main(new[] { "-ci" });
        }

        [Ignore("Use for bulk replication")]
        [Test]
        public void StatisticsReplicationTest()
        {
            Program.Main(new[] { "-statistics" });
        }
    }
}
